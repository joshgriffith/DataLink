using System.Collections;
using System.Threading.Tasks;

namespace DataLink.Core.Interceptors  {
    public interface IsSaveInterceptor {
        Task BeforeSave(IEnumerable entities);
        Task AfterSave(IEnumerable entities);
    }
}