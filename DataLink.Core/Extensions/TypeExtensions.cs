using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DataLink.Core.Extensions {
    public static class TypeExtensions {

        public static bool IsDotNetType(this Type type) {
            return type.Assembly.IsDotNetAssembly();
        }

        public static bool IsValueTypeOrString(this Type type) {
            return type.IsValueType || type == typeof(string);
        }

        public static bool IsEnumerable(this Type type) {
            return !type.IsValueTypeOrString() && type.IsAssignableTo(typeof(IEnumerable));
        }

        public static object InvokeStaticGenericMethod(this Type type, string methodName, Type genericType, params object[] arguments) {
            return type.GetMethod(methodName).MakeGenericMethod(genericType).Invoke(null, arguments);
        }

        public static object InvokeStaticMethod(this Type type, string methodName, params object[] arguments) {
            return type.GetMethod(methodName).Invoke(null, arguments);
        }

        public static object InvokeExtensionMethod(this Type type, string methodName, object target, Type genericType = null, params object[] arguments) {
            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            var args = new[] { target }.Concat(arguments).ToArray();

            MethodInfo match = null;

            foreach (var method in methods.Where(each => each.Name.ToLower() == methodName.ToLower())) {
                if ((genericType == null) == method.IsGenericMethod)
                    continue;

                if (method.GetParameters().Length != args.Length)
                    continue;

                match = method;
            }

            if (match == null)
                throw new Exception("Cannot find extension method: " + methodName);

            if (genericType != null)
                match = match.MakeGenericMethod(genericType);

            return match.Invoke(null, args);
        }

        public static bool HasInterface(this Type type, Type interfaceType) {
            if (interfaceType.IsGenericType)
                return type.GetInterfaces().Any(each => each.IsGenericType && each.GetGenericTypeDefinition() == interfaceType);

            return interfaceType.IsAssignableFrom(type);
        }

        public static bool Implements(this Type type, Type interfaceOrBaseType) {
            if (type == interfaceOrBaseType)
                return true;

            return type.GetBaseTypes().Any(each => each == interfaceOrBaseType);
        }

        public static List<Type> GetBaseTypes(this Type type) {
            var types = type.GetInterfaces().ToList();

            if (type.BaseType != null && !types.Contains(type.BaseType))
                types.Add(type.BaseType);

            foreach (var eachBaseType in types.ToList()) {
                var parentTypes = eachBaseType.GetBaseTypes();

                foreach (var parentType in parentTypes)
                    if (!types.Contains(parentType))
                        types.Add(parentType);
            }

            return types;
        }

        public static object New(this Type type, params object[] parameters) {
            return Activator.CreateInstance(type, parameters);
        }

        public static MethodInfo GetGenericMethod(this Type type, string methodName, Type genericType) {
            var methods = type.GetMethods(BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            var method = methods.First(each => each.Name == methodName && each.IsGenericMethod);
            return method.MakeGenericMethod(genericType);
        }

        public static bool IsSimpleType(this Type type) {
            if (type == typeof(object))
                return false;

            if (!type.IsEnum && !type.FullName.StartsWith(nameof(System) + "."))
                return false;

            if (type.IsGenericType && type.GetEnumerableType() != null)
                return false;

            return true;
        }

        public static Type GetEnumerableType(this Type type) {
            var enumerable = FindEnumerable(type);

            if (enumerable != null)
                return enumerable.GetTypeInfo().GetGenericArguments()[0];

            return null;
        }

        private static Type FindEnumerable(this Type type) {
            if (type == null || type == typeof(string))
                return null;

            var seqTypeInfo = type.GetTypeInfo();

            if (seqTypeInfo.IsGenericType && seqTypeInfo.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                return type;

            if (seqTypeInfo.IsArray)
                return typeof(IEnumerable<>).MakeGenericType(type.GetElementType());

            if (seqTypeInfo.IsGenericType) {
                foreach (var arg in type.GetTypeInfo().GetGenericArguments()) {
                    var enumerable = typeof(IEnumerable<>).MakeGenericType(arg);

                    if (enumerable.GetTypeInfo().IsAssignableFrom(type))
                        return enumerable;
                }
            }

            var interfaces = seqTypeInfo.GetInterfaces();

            if (interfaces.Length > 0) {
                foreach (var each in interfaces) {
                    var enumerable = FindEnumerable(each);

                    if (enumerable != null)
                        return enumerable;
                }
            }

            if (seqTypeInfo.BaseType != null && seqTypeInfo.BaseType != typeof(object))
                return FindEnumerable(seqTypeInfo.BaseType);

            return null;
        }
    }
}