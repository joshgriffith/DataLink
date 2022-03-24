using System;
using System.Linq.Expressions;

namespace DataLink.Core.Filters {
    internal class DefaultQueryFilter<T> : QueryFilter<T> {
        public Expression<Func<T, bool>> FilterExpression { get; set; }

        public DefaultQueryFilter() {
        }

        public DefaultQueryFilter(Expression<Func<T, bool>> expression, QueryFilterTypes type = QueryFilterTypes.Inclusive) {
            FilterExpression = expression;
            Type = type;
        }

        protected override Expression<Func<T, bool>> GetFilterExpression() {
            return FilterExpression;
        }
    }
}