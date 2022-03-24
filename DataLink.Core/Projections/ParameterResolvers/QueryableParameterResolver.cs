using System;
using System.Linq;
using System.Threading.Tasks;
using DataLink.Core.Extensions;

namespace DataLink.Core.Projections.ParameterResolvers {
    public class QueryableParameterResolver : IsTypeResolver {
        private readonly DataHub _provider;

        public QueryableParameterResolver(DataHub provider) {
            _provider = provider;
        }

        public async Task<object> ResolveAsync(Type type) {
            return _provider.Get(type.GetGenericArguments().First());
        }

        public bool CanResolve(Type type) {
            return type.Implements(typeof(IQueryable)) && type.GetGenericArguments().Length == 1;
        }
    }
}