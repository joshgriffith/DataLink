using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DataLink.Core.Utilities {
    public class ExpressionPathBuilder : ExpressionVisitor {
        private readonly List<string> _segments = new();
        
        protected override Expression VisitMember(MemberExpression node) {
            var result = base.VisitMember(node);
            _segments.Add(node.Member.Name);
            return result;
        }

        public IEnumerable<string> GetSegments() {
            return _segments;
        }

        public ExpressionPath GetPath() {
            return new ExpressionPath(_segments.ToArray());
        }
    }
}