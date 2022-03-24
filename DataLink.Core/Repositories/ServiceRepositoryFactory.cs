using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataLink.Core.ChangeTracking;

namespace DataLink.Core.Repositories {
    public class ServiceRepositoryFactory<T> : IsRepository {

        private readonly DataHub _provider;
        private readonly Func<DataHub, IQueryable<T>> _accessor;
        
        public ServiceRepositoryFactory(DataHub provider, Func<DataHub, IQueryable<T>> accessor) {
            _provider = provider;
            _accessor = accessor;
        }

        public IQueryable<T> Get() {
            return _accessor(_provider);
        }

        public IQueryable<X> Get<X>() {
            return (IQueryable<X>) _accessor(_provider);
        }

        public Task CommitAsync(ChangeSet changeset) {
            return Task.CompletedTask;
        }

        public IEnumerable<Type> GetEntityTypes() {
            return new List<Type> { typeof(T) };
        }
    }
}