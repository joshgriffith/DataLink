using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataLink.Core.ChangeTracking;

namespace DataLink.Core.Repositories {
    public interface IsRepository {
        IQueryable<T> Get<T>();
        Task CommitAsync(ChangeSet changeset);
        IEnumerable<Type> GetEntityTypes();
    }
}