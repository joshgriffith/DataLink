using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DataLink.Core.ChangeTracking.Graph {
    public class ObjectGraphCollection : ObjectGraphNode {
        public List<ObjectGraphNode> Items { get; set; }

        public ObjectGraphCollection(string name, IEnumerable collection)
            : base(name, collection) {
            Items = new List<ObjectGraphNode>();

            if (collection != null) {
                if (collection is IDictionary<string, object> dictionary) {
                    foreach (var item in dictionary) {
                        Items.Add(From(item.Key, item.Value.GetType(), item.Value));
                    }
                }
                else {
                    var index = 0;

                    foreach (var item in collection) {
                        Items.Add(From(index.ToString(), item.GetType(), item));
                        index += 1;
                    }
                }
            }
        }

        public override IEnumerable<ChangedValue> GetChanges(object currentValue = null) {
            var current = new ObjectGraphCollection(string.Empty, currentValue as IEnumerable);
            
            foreach (var item in Items) {
                if (current.Items.All(each => !each.Value.Equals(item.Value))) {
                    yield return new ChangedValue {
                        Value = item.Value,
                        Path = Name,
                        Type = ChangeTypes.Delete
                    };
                }
                else {
                    if (item.GetChanges().Any()) {
                        yield return new ChangedValue {
                            Value = item.Value,
                            Path = Name,
                            Type = ChangeTypes.Update
                        };
                    }
                }
            }

            foreach (var item in current.Items) {
                if (Items.All(each => each.Value != item.Value)) {
                    yield return new ChangedValue {
                        Value = item.Value,
                        Path = Name,
                        Type = ChangeTypes.Create
                    };
                }
            }
        }
    }
}