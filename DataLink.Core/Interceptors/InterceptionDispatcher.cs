using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataLink.Core.ChangeTracking;

namespace DataLink.Core.Interceptors {
    public class InterceptionDispatcher : IsCommitInterceptor {
        private readonly IEnumerable<IsSaveInterceptor> _saves;
        private readonly IEnumerable<IsDeleteInterceptor> _deletes;

        public InterceptionDispatcher(IEnumerable<IsSaveInterceptor> saves, IEnumerable<IsDeleteInterceptor> deletes) {
            _saves = saves;
            _deletes = deletes;
        }

        public async Task BeforeCommit(ChangeSet changes) {
            await Dispatch(changes,
                (interceptor, entities) => interceptor.BeforeSave(entities),
                (interceptor, entities) => interceptor.BeforeDelete(entities)
            );
        }

        public async Task AfterCommit(ChangeSet changes) {
            await Dispatch(changes,
                (interceptor, entities) => interceptor.AfterSave(entities),
                (interceptor, entities) => interceptor.AfterDelete(entities)
            );
        }

        private async Task Dispatch(ChangeSet changes, Func<IsSaveInterceptor, List<object>, Task> saveCallback, Func<IsDeleteInterceptor, List<object>, Task> deleteCallback) {

            var saved = changes.Entries
                .Where(each => each.Status is EntityStatusTypes.Added or EntityStatusTypes.Modified)
                .Select(each => each.Value)
                .ToList();

            var deleted = changes.Entries
                .Where(each => each.Status == EntityStatusTypes.Deleted)
                .Select(each => each.Value)
                .ToList();

            if (saved.Any())
                foreach (var interceptor in _saves)
                    await saveCallback(interceptor, saved);

            if (deleted.Any())
                foreach (var interceptor in _deletes)
                    await deleteCallback(interceptor, deleted);
        }
    }
}