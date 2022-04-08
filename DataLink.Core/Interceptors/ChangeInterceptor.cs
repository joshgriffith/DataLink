using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DataLink.Core.ChangeTracking;
using DataLink.Core.Utilities;

namespace DataLink.Core.Interceptors {
    public class ChangeInterceptor<T> : IsCommitInterceptor {

        private readonly Dictionary<string, List<IsChangeInvoker>> _listeners = new();

        public async Task BeforeCommit(ChangeSet changes) {
            foreach (var entry in changes.Entries.Where(each => each.Value is T)) {
                foreach (var group in _listeners) {
                    if (entry.HasChange(group.Key)) {
                        foreach (var listener in group.Value) {
                            foreach (var change in entry.GetChanges(group.Key, listener.Type)) {
                                await listener.InvokeAsync(entry.Value, change.Value);
                            }
                        }
                    }
                }
            }
        }

        public async Task AfterCommit(ChangeSet changes) {
            foreach (var entry in changes.Entries.Where(each => each.Value is T)) {
                foreach (var group in _listeners) {
                    if (entry.HasChange(group.Key)) {
                        foreach (var listener in group.Value) {
                            foreach (var change in entry.GetChanges(group.Key, listener.Type)) {
                                await listener.InvokeAsync(entry.Value, change.Value);
                            }
                        }
                    }
                }
            }
        }

        public void On<X>(Expression<Func<T, X>> accessor, Func<T, X, Task> handler) {
            AddListener(accessor, new ChangeInvoker<X>(handler, ChangeTypes.Save));
        }

        /*public void OnAdd(Func<T, IEnumerable, object, Task> handler) {
            throw new NotImplementedException();
        }*/

        public void OnAdd<X>(Expression<Func<T, List<X>>> accessor, Func<T, X, Task> handler) {
            AddListener(accessor, new ChangeInvoker<X>(handler, ChangeTypes.Save));
        }

        /*public void OnSave<X>(Expression<Func<T, List<X>>> accessor, Func<T, X, Task> handler) {
            AddListener(accessor, new ChangeInvoker<X>(handler, ChangeTypes.Save));
        }*/

        public void OnRemove<X>(Expression<Func<T, List<X>>> accessor, Func<T, X, Task> handler) {
            AddListener(accessor, new ChangeInvoker<X>(handler, ChangeTypes.Delete));
        }

        private void AddListener<X>(Expression expression, ChangeInvoker<X> invoker) {
            var path = GetPath(expression).Path;

            if(!_listeners.ContainsKey(path))
                _listeners.Add(path, new List<IsChangeInvoker>());

            _listeners[path].Add(invoker);
        }

        private ExpressionPath GetPath(Expression expression) {
            var builder = new ExpressionPathBuilder();
            builder.Visit(expression);
            return builder.GetPath();
        }

        protected interface IsChangeInvoker {
            Task InvokeAsync(object source, object value);
            ChangeTypes Type { get; set; }
        }

        protected class ChangeInvoker<X> : IsChangeInvoker {
            public ChangeTypes Type { get; set; }

            private Func<T, X, Task> Handler { get; set; }

            public ChangeInvoker(Func<T, X, Task> handler, ChangeTypes type) {
                Handler = handler;
                Type = type;
            }

            public async Task InvokeAsync(object source, object value) {
                await Handler((T) source, (X) value);
            }
        }
    }
}