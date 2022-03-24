using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DataLink.Core.Projections {
    public class ProjectionMapping {
        public IEnumerable<Type> ParameterTypes { get; }
        public Type OutputType { get; }
        private readonly MethodInfo _method;

        public ProjectionMapping(MethodInfo method) {
            _method = method;
            ParameterTypes = method.GetParameters().Select(each => each.ParameterType).ToList();
            OutputType = method.ReturnType.GenericTypeArguments.First();
        }

        public IQueryable<T> Get<T>(params object[] parameters) {
            return _method.Invoke(null, parameters) as IQueryable<T>;
        }
    }
}