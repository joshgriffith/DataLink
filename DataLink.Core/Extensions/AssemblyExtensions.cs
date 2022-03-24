using System.Reflection;

namespace DataLink.Core.Extensions {
    public static class AssemblyExtensions {
        public static bool IsDotNetAssembly(this Assembly assembly) {
            return assembly.FullName.StartsWith(nameof(System)) || assembly.FullName.StartsWith(nameof(Microsoft));
        }
    }
}