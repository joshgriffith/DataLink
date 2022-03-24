using System.Collections;

namespace DataLink.Core.Interceptors {
    public interface IsLoadInterceptor {
        void OnLoad(IEnumerable entities);
    }
}