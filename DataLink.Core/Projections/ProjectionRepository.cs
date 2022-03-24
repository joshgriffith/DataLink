using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataLink.Core.ChangeTracking;
using DataLink.Core.Exceptions;
using DataLink.Core.Projections.ParameterResolvers;
using DataLink.Core.Repositories;

namespace DataLink.Core.Projections {
    public class ProjectionRepository : IsRepository {

        private readonly IServiceProvider _services;
        private readonly ProjectionMapper _mapper;
        private readonly List<IsTypeResolver> _resolvers;

        public ProjectionRepository(IServiceProvider services, ProjectionMapper mapper, IEnumerable<IsTypeResolver> resolvers) {
            _services = services;
            _mapper = mapper;
            _resolvers = resolvers.ToList();
        }

        public IQueryable<T> Get<T>() {
            var mapping = _mapper.Get<T>();
            var parameters = new List<object>();

            foreach (var parameter in mapping.ParameterTypes) {
                var resolver = _resolvers.FirstOrDefault(each => each.CanResolve(parameter));

                if (resolver == null)
                    throw new MappingException(parameter);
            }

            var query = mapping.Get<T>(parameters);
            //var provider = new ServiceMethodQueryProvider(_services);
            //return provider.CreateQuery<T>()

            return mapping.Get<T>(parameters);
        }

        public Task CommitAsync(ChangeSet changeset) {
            return null;
        }

        public IEnumerable<Type> GetEntityTypes() {
            return _mapper.GetProjectionTypes();
        }
    }
}