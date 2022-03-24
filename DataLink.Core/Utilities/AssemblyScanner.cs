using System;
using System.Collections.Generic;
using System.Reflection;
using DataLink.Core.Extensions;

namespace DataLink.Core.Utilities {
    public static class AssemblyScanner {
        public static IEnumerable<Assembly> Scan(bool includeDotNetAssemblies = false) {
            return Scan(_ => true, includeDotNetAssemblies);
        }

        public static IEnumerable<Assembly> Scan(Func<Assembly, bool> predicate, bool includeDotNetAssemblies = false) {
            var results = new List<string>();
            var stack = new Stack<Assembly>();

            stack.Push(Assembly.GetEntryAssembly());

            do {
                var assembly = stack.Pop();

                if (!includeDotNetAssemblies && assembly.IsDotNetAssembly())
                    continue;

                if (!predicate(assembly))
                    continue;

                yield return assembly;

                foreach (var reference in assembly.GetReferencedAssemblies()) {
                    if (!results.Contains(reference.FullName)) {
                        stack.Push(Assembly.Load(reference));
                        results.Add(reference.FullName);
                    }
                }
            } while (stack.Count > 0);
        }
    }
}