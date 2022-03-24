using System.Collections.Generic;
using System.Linq;

namespace DataLink.Core.ChangeTracking {
    public class ChangeSetItem {
        public object Value { get; set; }
        public Dictionary<string, List<ChangedValue>> Changes { get; set; } = new();
        public EntityStatusTypes Status { get; set; }
        
        public ChangeSetItem(object value, List<ChangedValue> changes, EntityStatusTypes status) {
            Value = value;
            Status = status;

            foreach (var change in changes) {
                if(!Changes.ContainsKey(change.Path))
                    Changes.Add(change.Path, new List<ChangedValue>());

                Changes[change.Path].Add(change);
            }
        }

        public bool HasChange(string key) {
            return Changes.ContainsKey(key);
        }

        public IEnumerable<ChangedValue> GetChanges(string key, ChangeTypes type) {
            return Changes.ContainsKey(key) ? Changes[key].Where(each => type.HasFlag(each.Type)) : null;
        }

        public IEnumerable<ChangedValue> GetChanges(ChangeTypes type) {
            return Changes.Values.SelectMany(each => each).Where(each => type.HasFlag(each.Type));
        }
    }
}