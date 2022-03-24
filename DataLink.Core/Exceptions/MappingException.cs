using System;

namespace DataLink.Core.Exceptions {
    public class MappingException : Exception {
        public Type Type { get; set; }

        public MappingException(Type type)
            : base($"No mappings found for type '{type.Name}'") {
            Type = type;
        }
    }
}