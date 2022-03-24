using System;
using System.Collections;
using System.Collections.Generic;
using DataLink.Core.Extensions;

namespace DataLink.Core.ChangeTracking.Graph {
    public class ObjectGraphNode {
        public string Name { get; set; }
        public object Value { get; set; }

        public ObjectGraphNode(string name, object value) {
            Name = name;
            Value = value;
        }

        public static ObjectGraphNode From(string name, Type type, object value) {
            if (type.IsValueTypeOrString())
                return new ObjectGraphNode(name, value);

            if (type.IsEnumerable())
                return new ObjectGraphCollection(name, value as IEnumerable);

            return new ObjectGraph(name, value);
        }

        public virtual IEnumerable<ChangedValue> GetChanges(object currentValue = null) {
            if (Value == null && currentValue == null)
                yield break;

            if (Value == null || currentValue == null || !Value.Equals(currentValue)) {
                yield return new ChangedValue {
                    Value = currentValue,
                    Path = Name,
                    Type = ChangeTypes.Update
                };
            }
        }

        public override string ToString() {
            return Name;
        }
    }
}