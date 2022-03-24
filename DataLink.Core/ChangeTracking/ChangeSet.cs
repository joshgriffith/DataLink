using System;
using System.Collections.Generic;
using System.Linq;

namespace DataLink.Core.ChangeTracking {
    public class ChangeSet {
        public List<ChangeSetItem> Entries { get; set; } = new();

        public static ChangeSet From(object object1, object object2) {
            if (object1 == null || object2 == null || object1.GetType() != object2.GetType())
                throw new ArgumentException("Incompatible object types.");

            var changeset = new ChangeSet();

            return changeset;
        }

        public IEnumerable<object> GetItems() {
            return Entries.Select(each => each.Value);
        }

        public IEnumerable<IGrouping<Type, object>> GetGroupedItems() {
            return GetItems().GroupBy(each => each.GetType());
        }

        public void Add(object item, List<ChangedValue> changes, EntityStatusTypes status) {
            var existing = Entries.FirstOrDefault(each => each.Value == item);

            if (existing != null)
                existing.Status = status;
            else
                Entries.Add(new ChangeSetItem(item, changes, status));
        }

        public bool Contains(object item) {
            return Entries.Any(each => each.Value == item);
        }
    }
}