using System.Collections;
using System.Linq;
using System.Threading.Tasks;

namespace DataLink.Core.Interceptors {
    public abstract class SaveInterceptor<T> : IsSaveInterceptor {

        public async Task BeforeSave(IEnumerable entities) {
            foreach(var entity in entities.OfType<T>())
                await OnBeforeSave(entity);
        }

        public async Task AfterSave(IEnumerable entities) {
            foreach(var entity in entities.OfType<T>())
                await OnAfterSave(entity);
        }

        protected virtual Task OnBeforeSave(T entity) {
            return Task.CompletedTask;
        }

        protected virtual Task OnAfterSave(T entity) {
            return Task.CompletedTask;
        }
    }
}