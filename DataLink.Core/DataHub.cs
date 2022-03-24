using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataLink.Core.ChangeTracking;
using DataLink.Core.Configuration;
using DataLink.Core.Exceptions;
using DataLink.Core.Extensions;
using DataLink.Core.Filters;
using DataLink.Core.Interceptors;
using DataLink.Core.Queries;
using DataLink.Core.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace DataLink.Core {
    public class DataHub {
        private readonly IServiceProvider _provider;
        
        public DataHub(IServiceProvider provider) {
            _provider = provider;
        }

        public static DataHub FromConfiguration(DataHubConfiguration configuration) {
            return configuration.BuildServiceProvider().GetRequiredService<DataHub>();
        }

        public IQueryable<T> Get<T>(bool useFilters = true) {
            var queryables = GetQueryables<T>().ToArray();

            if (!queryables.Any())
                throw new MappingException(typeof(T));

            var visitors = _provider.GetServices<QueryVisitor>();

            queryables = queryables.Select(query => {
                return visitors.Aggregate(query, (lastQuery, visitor) => visitor.Visit(lastQuery));
            }).ToArray();

            if (useFilters) {
                var filters = _provider.GetServices<QueryFilter>().ToArray();
                queryables = queryables.Select(query => QueryFilter.Visit(query, filters)).ToArray();
            }
            
            return new AggregatedQuery<T>(OnDataReceived, queryables);
        }

        private void OnDataReceived(IEnumerable results) {
            var interceptors = _provider.GetServices<IsLoadInterceptor>().ToArray();

            foreach(var interceptor in interceptors)
                interceptor.OnLoad(results);
        }

        public IQueryable<T> Get<T>(string typeName) {
            return Get(ResolveType(typeName)) as IQueryable<T>;
        }

        internal IQueryable Get(Type type) {
            return GetType().GetGenericMethod(nameof(Get), type).Invoke(this, null) as IQueryable;
        }

        public Type ResolveType(string typeName) {
            typeName = typeName.ToLower();

            return _provider.GetServices<IsRepository>()
                .SelectMany(each => each.GetEntityTypes())
                .FirstOrDefault(each => each.Name.ToLower() == typeName);
        }

        public void Add<T>(T entity) {
            if (entity == null)
                return;

            var changetracker = _provider.GetRequiredService<IsChangeTracker>();
            changetracker.Add(entity);
        }
        
        public async Task<T> SaveAsync<T>(params T[] entities) {
            if (!entities.Any())
                return default;

            var changetracker = _provider.GetRequiredService<IsChangeTracker>();

            foreach(var entity in entities)
                changetracker.Add(entity, EntityStatusTypes.Modified);

            await CommitAsync();

            return entities.FirstOrDefault();
        }

        public async Task<bool> CommitAsync() {
            var changetracker = _provider.GetRequiredService<IsChangeTracker>();
            var changeset = changetracker.GetChanges();
            var groups = changeset.GetGroupedItems();
            var hasChanges = false;
            
            // Todo:  Parallelized transaction handling
            foreach (var group in groups) {
                var repository = GetRepositoryFor(group.Key);

                if (await CommitAsync(repository, changeset)) {
                    hasChanges = true;
                }
            }

            return hasChanges;
        }

        private async Task<bool> CommitAsync(IsRepository repository, ChangeSet changeset) {

            if (!changeset.Entries.Any())
                return false;
            
            var interceptors = _provider.GetServices<IsCommitInterceptor>().ToList();
            var changetracker = _provider.GetRequiredService<IsChangeTracker>();

            foreach (var interceptor in interceptors)
                await interceptor.BeforeCommit(changeset);

            changeset = changetracker.GetChanges();

            await repository.CommitAsync(changeset);

            foreach (var interceptor in interceptors)
                await interceptor.AfterCommit(changeset);

            return true;
        }

        private IsRepository GetRepositoryFor(Type type) {
            return _provider.GetServices<IsRepository>().FirstOrDefault(repository => repository.GetEntityTypes().Any(each => each.Implements(type)));
        }
        
        private IEnumerable<IQueryable<T>> GetQueryables<T>() {
            foreach (var repository in _provider.GetServices<IsRepository>()) {
                foreach (var type in repository.GetEntityTypes().Where(each => each.Implements(typeof(T)))) {
                    var method = typeof(IsRepository).GetGenericMethod(nameof(IsRepository.Get), type);
                    yield return method.Invoke(repository, null) as IQueryable<T>;
                }
            }
        }
    }
}