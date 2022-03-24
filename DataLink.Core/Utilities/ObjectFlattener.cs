using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using DataLink.Core.Extensions;

namespace DataLink.Core.Utilities {
    public static class ObjectFlattener {
        private static readonly ConcurrentDictionary<Type, Dictionary<PropertyInfo, Func<object, object>>> _propertyCache = new();
        
        public static Dictionary<string, string> Flatten(object target, string prefix = "", bool includeDefaultProperties = false) {
            return Flatten(target, new Dictionary<string, string>(), prefix, includeDefaultProperties);
        }

        private static Dictionary<string, string> Flatten(object target, Dictionary<string, string> dictionary = default, string prefix = "", bool includeDefaultProperties = false) {
            dictionary ??= new Dictionary<string, string>();

            foreach (var (property, accessor) in GetProperties(target.GetType())) {
                var key = string.IsNullOrWhiteSpace(prefix) ? property.Name : $"{prefix}.{property.Name}";
                var value = accessor(target);

                if (value == null) {
                    if(includeDefaultProperties)
                        dictionary.Add(key, Serialize(property.PropertyType, null));

                    continue;
                }

                dictionary.Add(key, Serialize(property.PropertyType, value));

                if (property.PropertyType.IsValueTypeOrString())
                    continue;
                
                if (value is IEnumerable enumerable) {
                    var counter = 0;

                    foreach (var item in enumerable) {
                        var itemKey = $"{key}[{counter++}]";
                        var itemType = item.GetType();

                        if (itemType.IsValueTypeOrString())
                            dictionary.Add(itemKey, Serialize(itemType, item));
                        else
                            Flatten(item, dictionary, itemKey);
                    }
                }
                else
                    Flatten(value, dictionary, key);
            }

            return dictionary;
        }

        private static string Serialize(Type type, object value) {
            if (value == null)
                return string.Empty;
            
            if (!type.IsValueTypeOrString() && value is IEnumerable enumerable)
                return enumerable.OfType<object>().Count().ToString();

            return value.ToString();
        }

        private static Dictionary<PropertyInfo, Func<object, object>> GetProperties(Type type) {
            if (_propertyCache.TryGetValue(type, out var properties))
                return properties;

            CacheProperties(type);
            return _propertyCache[type];
        }

        private static void CacheProperties(Type type) {
            if (_propertyCache.ContainsKey(type))
                return;

            _propertyCache[type] = new Dictionary<PropertyInfo, Func<object, object>>();
            
            foreach (var propertyInfo in type.GetProperties().Where(x => x.CanRead)) {

                var getter = GetAccessor(propertyInfo);
                _propertyCache[type].Add(propertyInfo, getter);

                if (!propertyInfo.PropertyType.IsValueTypeOrString()) {
                    if (propertyInfo.PropertyType.IsEnumerable()) {
                        var types = propertyInfo.PropertyType.GetGenericArguments();

                        foreach (var genericType in types) {
                            if (!genericType.IsValueTypeOrString())
                                CacheProperties(genericType);
                        }
                    }
                    else {
                        CacheProperties(propertyInfo.PropertyType);
                    }
                }
            }
        }
        
        private static Func<object, object> GetAccessor(PropertyInfo property) {
            var objectParameter = Expression.Parameter(typeof(object));
            var castExpression = Expression.TypeAs(objectParameter, property.DeclaringType);
            var convertExpression = Expression.Convert(Expression.Property(castExpression, property), typeof(object));
            return Expression.Lambda<Func<object, object>>(convertExpression, objectParameter).Compile();
        }
    }
}