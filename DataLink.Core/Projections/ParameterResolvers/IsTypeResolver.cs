using System;
using System.Threading.Tasks;

namespace DataLink.Core.Projections.ParameterResolvers {
    public interface IsTypeResolver {
        Task<object> ResolveAsync(Type type);
        bool CanResolve(Type type);
    }
}