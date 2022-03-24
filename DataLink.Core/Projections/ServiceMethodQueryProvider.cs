using System;
using System.Linq;
using System.Linq.Expressions;

namespace DataLink.Core.Projections {
    public class ServiceMethodQueryProvider : IQueryProvider {

        private IServiceProvider _serviceProvider;

        public ServiceMethodQueryProvider(IServiceProvider serviceProvider) {
            _serviceProvider = serviceProvider;
        }

        public IQueryable CreateQuery(Expression expression) {
            return null;
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression) {
            return null;
        }

        public object? Execute(Expression expression) {
            return null;
        }

        public TResult Execute<TResult>(Expression expression) {
            return default;
        }
    }
}