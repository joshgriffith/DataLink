using System.Collections.Generic;

namespace DataLink.Core.ChangeTracking.Graph {
    
    public class ObjectGraph : ObjectGraphNode {
        public List<ObjectGraphNode> Properties { get; set; }

        public ObjectGraph(object value)
            : this(string.Empty, value) {
        }

        public ObjectGraph(string name, object value)
            : base(name, value) {
            Properties = new List<ObjectGraphNode>();

            if (value != null) {

                // Todo: Cache accessors
                foreach (var property in value.GetType().GetProperties()) {
                    var node = From(property.Name, property.PropertyType, property.GetValue(value));
                    Properties.Add(node);
                }
            }
        }

        public override IEnumerable<ChangedValue> GetChanges(object currentValue = null) {

            if (currentValue != null) {
                if ((Value == null && currentValue != Value) || !currentValue.Equals(Value)) {
                    yield return new ChangedValue {
                        Value = currentValue,
                        Path = Name,
                        Type = ChangeTypes.Update
                    };
                }
            }

            foreach (var property in Properties) {
                var current = Value.GetType().GetProperty(property.Name).GetValue(Value);

                foreach (var change in property.GetChanges(current)) {
                    yield return change;
                }
            }
        }
    }
}