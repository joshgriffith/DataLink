using System.Linq;
using System.Linq.Expressions;
using DataLink.Core.Extensions;

namespace DataLink.Core.Queries {
    public class AggregatedQueryRebinder : ExpressionVisitor {
        private readonly IQueryable _defaultQueryable;

        public AggregatedQueryRebinder(IQueryable defaultQueryable) {
            _defaultQueryable = defaultQueryable;
        }

        protected override Expression VisitConstant(ConstantExpression node) {
            if (node.Type.HasInterface(typeof(IsAggregatedQuery))) {
                var query = node.Value as IQueryable;

                if (_defaultQueryable != null && _defaultQueryable.ElementType.Implements(query.ElementType))
                    return _defaultQueryable.Expression;

                var provider = query.Provider as AggregatedQueryProvider;
                var queryable = provider.Queryables.First(each => each.ElementType == query.ElementType);
                
                var method = queryable.Expression as MethodCallExpression;
                return method.Arguments.First();
            }

            return base.VisitConstant(node);
        }
    }
}