using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DataLink.Core.Queries {
    public class AggregatedQuery<T> : IOrderedQueryable<T>, IsAggregatedQuery {
        public Type ElementType => typeof(T);
        public Expression Expression { get; }
        public IQueryProvider Provider { get; }

        public AggregatedQuery(Action<IEnumerable> callback, params IQueryable[] queries) {
            Provider = new AggregatedQueryProvider(callback, queries);
            Expression = Expression.Constant(this);
        }

        internal AggregatedQuery(IQueryProvider provider, Expression expression) {
            Provider = provider;
            Expression = expression;
        }

        public IEnumerator<T> GetEnumerator() {
            return Provider.Execute<IEnumerable<T>>(Expression).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}