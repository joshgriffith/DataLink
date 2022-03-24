using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DataLink.Core.Extensions;

namespace DataLink.Core.Utilities {
    public abstract class DictionaryVisitor {
        
        public void Visit(Dictionary<string, object> dictionary) {
            var mappings = new List<KeyValuePair<string, string>>();

            foreach (var pair in dictionary) {
                mappings.Add(new KeyValuePair<string, string>(pair.Key, VisitKey(pair.Key)));

                if (pair.Value is Dictionary<string, object> child)
                    Visit(child);
            }

            foreach (var mapping in mappings) {
                if (dictionary.ContainsKey(mapping.Value))
                    continue;

                dictionary[mapping.Value] = dictionary[mapping.Key];
                dictionary.Remove(mapping.Key);
            }
        }

        public void Visit(object value) {
            if (value is Dictionary<string, object> dictionary) {
                Visit(dictionary);
                return;
            }

            /*if (value is IEnumerable enumerable && !enumerable.GetType().GetGenericArguments().First().IsSimpleType()) {
                foreach (var item in enumerable) {
                    Visit(item);
                }
            }*/

            foreach (var property in value.GetType().GetProperties())
                VisitValue(property.GetValue(value));

            foreach (var field in value.GetType().GetFields())
                VisitValue(field.GetValue(value));
        }

        private void VisitValue(object value) {
            if (value == null || value.GetType().IsSimpleType())
                return;

            if (value is Dictionary<string, object> dictionaryProperty) {
                Visit(dictionaryProperty);
                return;
            }

            if (value is IEnumerable enumerable) {
                var arg = enumerable.GetType().GetGenericArguments().First();

                if (!arg.IsSimpleType()) {
                    foreach (var item in enumerable) {
                        Visit(item);
                    }
                }
            }
        }

        protected virtual string VisitKey(string key) {
            return key;
        }
    }
}