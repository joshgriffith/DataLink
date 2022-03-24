using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DataLink.Core.Exceptions;
using DataLink.Core.Extensions;
using DataLink.Core.Utilities;

namespace DataLink.Core.Projections {
    public class ProjectionMapper {
        private readonly Dictionary<Type, ProjectionMapping> _mappings = new();

        public void Automap(Func<Assembly, bool> predicate) {
            ScanAssemblies(predicate);
        }

        public ProjectionMapping Get<T>() {
            if (!_mappings.ContainsKey(typeof(T)))
                throw new MappingException(typeof(T));

            return _mappings[typeof(T)];
        }
            
        internal IEnumerable<Type> GetProjectionTypes() {
            return _mappings.Keys;
        }

        private void ScanAssemblies(Func<Assembly, bool> predicate) {
            foreach (var assembly in AssemblyScanner.Scan(predicate)) {
                foreach (var type in assembly.GetTypes()) {
                    foreach (var method in type.GetMethods().Where(ShouldMapMethod)) {
                        var mapping = new ProjectionMapping(method);
                        _mappings.Add(mapping.OutputType, mapping);
                    }
                }
            }
        }
        
        private bool ShouldMapMethod(MethodInfo method) {

            if (method.GetCustomAttribute<ProjectionAttribute>() == null)
                return false;

            if (!method.ReturnType.Implements(typeof(IQueryable)))
                return false;

            return true;
        }
    }
}