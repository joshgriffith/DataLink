using System.Collections;
using System.Linq;
using System.Threading.Tasks;

namespace DataLink.Core.Interceptors {
    public abstract class DeleteInterceptor<T> : IsDeleteInterceptor {

        public async Task BeforeDelete(IEnumerable entities) {
            foreach(var entity in entities.OfType<T>())
                await OnBeforeDelete(entity);
        }

        public async Task AfterDelete(IEnumerable entities) {
            foreach(var entity in entities.OfType<T>())
                await OnAfterDelete(entity);
        }

        protected virtual Task OnBeforeDelete(T entity) {
            return Task.CompletedTask;
        }

        protected virtual Task OnAfterDelete(T entity) {
            return Task.CompletedTask;
        }
    }
}