using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DataLink.Core.Extensions;
using DataLink.Core.Utilities;

namespace DataLink.Core.Filters {
    public abstract class QueryFilter {
        public QueryFilterTypes Type { get; set; }

        public abstract Type GetQueryType();
        public abstract Expression<Func<X, bool>> GetFilterExpression<X>();

        public static IQueryable Visit(IQueryable query, QueryFilter[] filters) {
            return typeof(QueryFilter).InvokeStaticGenericMethod(nameof(Visit), query.ElementType, query, filters) as IQueryable;
        }

        public static IQueryable<T> Visit<T>(IQueryable<T> query, QueryFilter[] filters) {

            filters = GetApplicableFilters(query.ElementType, filters).ToArray();

            if (filters.Any()) {
                var predicate = CompileFilterPredicate<T>(filters);
                return query.Where(predicate);
            }

            return query;
        }

        private static IEnumerable<QueryFilter> GetApplicableFilters(Type type, QueryFilter[] filters) {
            return filters.Where(filter => type.Implements(filter.GetQueryType()));
        }

        private static Expression<Func<T, bool>> CompileFilterPredicate<T>(QueryFilter[] filters) {
            
            Expression<Func<T, bool>> predicate = null;

            var exclusive = filters.Where(each => each.Type == QueryFilterTypes.Exclusive).Select(each => each.GetFilterExpression<T>()).ToList();
            var inclusive = filters.Where(each => each.Type == QueryFilterTypes.Inclusive).Select(each => each.GetFilterExpression<T>()).ToList();

            if (exclusive.Any())
                predicate = exclusive.AndAll();

            if (inclusive.Any()) {
                if (predicate != null) {
                    var expression = Expression.AndAlso(predicate.Body, predicate.TransferParameters(inclusive.OrAny()).Body);
                    return Expression.Lambda<Func<T, bool>>(expression, predicate.Parameters);
                }
                
                return inclusive.OrAny();
            }

            return predicate;
        }
    }

    public abstract class QueryFilter<T> : QueryFilter {

        protected QueryFilter()
            : this(QueryFilterTypes.Inclusive) {
        }

        protected QueryFilter(QueryFilterTypes type) {
            Type = type;
        }

        public override Type GetQueryType() {
            return typeof(T);
        }

        protected abstract Expression<Func<T, bool>> GetFilterExpression();

        public override Expression<Func<X, bool>> GetFilterExpression<X>() {
            var expression = GetFilterExpression().Convert<T, X>();
            return Type == QueryFilterTypes.Inclusive ? expression : expression.Negate();
        }
    }
}