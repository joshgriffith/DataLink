using System.Collections;
using System.Threading.Tasks;

namespace DataLink.Core.Interceptors {
    public interface IsDeleteInterceptor {
        Task BeforeDelete(IEnumerable entities);
        Task AfterDelete(IEnumerable entities);
    }
}