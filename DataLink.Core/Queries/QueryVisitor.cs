using System.Linq;

namespace DataLink.Core.Queries {
    public abstract class QueryVisitor {
        public abstract IQueryable<T> Visit<T>(IQueryable<T> query);
    }
}