using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DataLink.Core.Utilities {
    public static class ExpressionConverter {
        
        public static Expression<Func<TTo, bool>> Convert<TFrom, TTo>(this Expression<Func<TFrom, bool>> from) {
            if (from == null)
                return null;

            return InnerConvert<Func<TFrom, bool>, Func<TTo, bool>>(from);
        }

        private static Expression<TTo> InnerConvert<TFrom, TTo>(Expression<TFrom> from)
            where TFrom : class
            where TTo : class {

            var fromTypes = from.Type.GetGenericArguments();
            var toTypes = typeof (TTo).GetGenericArguments();

            if (fromTypes.Length != toTypes.Length)
                throw new NotSupportedException("Incompatible lambda function-type signatures");

            var typeMap = new Dictionary<Type, Type>();

            for (var i = 0; i < fromTypes.Length; i++)
                typeMap[fromTypes[i]] = toTypes[i];
            
            var parameterMap = new Dictionary<Expression, Expression>();
            var newParams = GenerateParameterMap(from, typeMap, parameterMap);
            
            var body = new TypeConversionVisitor<TTo>(parameterMap).Visit(from.Body);
            return Expression.Lambda<TTo>(body, newParams);
        }

        private static ParameterExpression[] GenerateParameterMap<TFrom>(
            Expression<TFrom> from,
            Dictionary<Type, Type> typeMap,
            Dictionary<Expression, Expression> parameterMap
        ) where TFrom : class {
            var newParams = new ParameterExpression[from.Parameters.Count];

            for (var i = 0; i < newParams.Length; i++) {
                var parameterName = "x" + (i + 1);

                if (typeMap.TryGetValue(from.Parameters[i].Type, out var newType)) {
                    parameterMap[from.Parameters[i]] = newParams[i] = Expression.Parameter(newType, parameterName);
                }
            }

            return newParams;
        }

        private class TypeConversionVisitor<T> : ExpressionVisitor {
            private readonly Dictionary<Expression, Expression> _parameterMap;

            public TypeConversionVisitor(Dictionary<Expression, Expression> parameterMap) {
                _parameterMap = parameterMap;
            }

            protected override Expression VisitParameter(ParameterExpression node) {
                if (!_parameterMap.TryGetValue(node, out var found))
                    found = base.VisitParameter(node);

                return found;
            }

            public override Expression Visit(Expression node) {
                if (node is LambdaExpression lambda && !_parameterMap.ContainsKey(lambda.Parameters.First()))
                    return new TypeConversionVisitor<T>(_parameterMap).Visit(lambda.Body);

                return base.Visit(node);
            }

            protected override Expression VisitMember(MemberExpression node) {

                var expression = Visit(node.Expression);

                if (expression != null && expression.Type != node.Type) {
                    if (expression.Type.GetMember(node.Member.Name).Any()) {
                        var newMember = expression.Type.GetMember(node.Member.Name).Single();
                        return Expression.MakeMemberAccess(expression, newMember);
                    }
                }

                return base.VisitMember(node);
            }
        }
    }
}