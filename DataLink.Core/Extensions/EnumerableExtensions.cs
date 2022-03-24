using System;
using System.Linq;

namespace DataLink.Core.Extensions {
    public static class EnumerableExtensions {
        
        public static Type GetGenericType(this Type type) {
            if (type.GenericTypeArguments.Any())
                return type.GenericTypeArguments.FirstOrDefault();

            return type.BaseType.GenericTypeArguments.FirstOrDefault();
        }
        
        public static Type GetGenericType(this object target) {
            return target.GetType().GetGenericType();
        }
    }
}