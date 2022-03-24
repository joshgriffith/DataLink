using System;
using System.Linq;
using System.Linq.Expressions;

namespace DataLink.Core.Utilities {
    public class TypeConversionExpressionVisitor : ExpressionVisitor {

        private ParameterExpression _parameter;
        private readonly Type _type;
        private readonly Type _fromType;

        public TypeConversionExpressionVisitor(Type toType, Type fromType = null) {
            _type = toType;
            _fromType = fromType;
        }

        private ParameterExpression GetParameterExpression(string name) {
            if(_parameter == null)
                _parameter = Expression.Parameter(_type, name);

            return _parameter;
        }

        protected override Expression VisitLambda<T>(Expression<T> node) {
            var expression = Visit(node.Body);

            var parameters = node.Parameters.Select(p => {
                if (p.Type.IsAssignableFrom(_type) || p.Type == _fromType)
                    return GetParameterExpression(p.Name);

                return p;
            }).ToList();
            
            return Expression.Lambda(expression, parameters);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node) {
            var arg1 = node.Arguments.First();
            var arg2 = node.Arguments.Last();

            var expression = Visit(arg2);
            var type = node.Type.GetGenericArguments().First();

            if (type.IsAssignableFrom(_type) || type == _fromType) {
                var genericMethod = node.Method.GetGenericMethodDefinition();
                var remapped = genericMethod.MakeGenericMethod(_type);
                return Expression.Call(null, remapped, arg1, expression);
            }

            return base.VisitMethodCall(node);
        }

        protected override Expression VisitMember(MemberExpression node) {
            var expression = Visit(node.Expression);

            if (node.Member.DeclaringType.IsAssignableFrom(_type) || node.Member.DeclaringType == _fromType) {
                var member = _type.GetMember(node.Member.Name).FirstOrDefault();

                if(member != null)
                    return Expression.MakeMemberAccess(expression, member);
            }

            return base.VisitMember(node);
        }

        protected override Expression VisitParameter(ParameterExpression node) {
            if (node.Type.IsAssignableFrom(_type) || node.Type == _fromType)
                return GetParameterExpression(node.Name);

            return base.VisitParameter(node);
        }
    }
}