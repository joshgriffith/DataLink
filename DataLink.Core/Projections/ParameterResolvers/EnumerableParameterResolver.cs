using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using DataLink.Core.Extensions;

namespace DataLink.Core.Projections.ParameterResolvers {
    public class EnumerableParameterResolver : IsTypeResolver {
        private readonly DataHub _provider;

        public EnumerableParameterResolver(DataHub provider) {
            _provider = provider;
        }

        public async Task<object> ResolveAsync(Type type) {
            return await _provider.Get(type.GetGenericArguments().First()).ToListAsync();
        }

        public bool CanResolve(Type type) {
            return type.Implements(typeof(IEnumerable)) && type.GetGenericArguments().Length == 1;
        }
    }
}