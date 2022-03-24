using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataLink.Core.ChangeTracking;

namespace DataLink.Core.Repositories {
    public class DefaultRepositoryFactory<T> : IsRepository {

        private readonly Func<IQueryable<T>> _accessor;
        
        public DefaultRepositoryFactory(Func<IQueryable<T>> accessor) {
            _accessor = accessor;
        }

        public IQueryable<T> Get() {
            return _accessor();
        }

        public IQueryable<X> Get<X>() {
            return (IQueryable<X>) _accessor();
        }

        public Task CommitAsync(ChangeSet changeset) {
            return Task.CompletedTask;
        }

        public IEnumerable<Type> GetEntityTypes() {
            return new List<Type> { typeof(T) };
        }
    }
}