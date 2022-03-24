using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DataLink.Core.Utilities;

namespace DataLink.Core.Extensions {
    public static class ExpressionExtensions {
        public static Expression<Func<T, bool>> Negate<T>(this Expression<Func<T, bool>> expression) {
            return Expression.Lambda<Func<T, bool>>(Expression.Not(expression.Body), expression.Parameters);
        }

        public static Expression<Func<T, bool>> AndAll<T>(this List<Expression<Func<T, bool>>> expressions) {
            
            if (expressions.Count <= 1)
                return expressions.FirstOrDefault();

            var mainExpression = expressions.First();
            var expression = expressions.Skip(1).Aggregate(mainExpression.Body, (previous, current) => Expression.AndAlso(mainExpression.TransferParameters(current).Body, previous));

            return Expression.Lambda<Func<T, bool>>(expression, mainExpression.Parameters);
        }

        public static Expression<Func<T, bool>> OrAny<T>(this List<Expression<Func<T, bool>>> expressions) {
            
            if (expressions.Count <= 1)
                return expressions.FirstOrDefault();
            
            var mainExpression = expressions.First();
            var expression = expressions.Skip(1).Aggregate(mainExpression.Body, (previous, current) => Expression.OrElse(mainExpression.TransferParameters(current).Body, previous));

            return Expression.Lambda<Func<T, bool>>(expression, mainExpression.Parameters);
        }

        public static T TransferParameters<T>(this T fromExpression, T toExpression) where T : LambdaExpression {
            var rebinder = new ExpressionNodeRebinder(toExpression.Parameters[0], fromExpression.Parameters[0]);
            return rebinder.Visit(toExpression) as T;
        }
    }
}