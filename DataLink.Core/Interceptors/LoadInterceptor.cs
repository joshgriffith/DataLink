using System.Collections;
using System.Linq;

namespace DataLink.Core.Interceptors {
    public abstract class LoadInterceptor<T> : IsLoadInterceptor {
        public void OnLoad(IEnumerable entities) {
            foreach(var entity in entities.OfType<T>())
                OnLoad(entity);
        }

        protected abstract void OnLoad(T entity);
    }
}