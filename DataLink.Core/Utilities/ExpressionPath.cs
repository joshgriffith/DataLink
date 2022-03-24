using System;

namespace DataLink.Core.Utilities {
    public struct ExpressionPath {
        public string[] Segments { get; set; }
        public string Path { get; set; }

        public ExpressionPath(params string[] segments) {
            Segments = segments;
            Path = string.Join('.', segments);
        }

        public override string ToString() {
            return Path;
        }
    }
}