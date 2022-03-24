using System;

namespace DataLink.Core.Utilities {
    public class CamelcaseDictionaryVisitor : DictionaryVisitor {
        protected override string VisitKey(string key) {
            if (string.IsNullOrEmpty(key))
                return string.Empty;

            return char.ToLowerInvariant(key[0]) + key[1..];
        }
    }
}