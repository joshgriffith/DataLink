using System;
using System.Linq;

namespace DataLink.Core.ChangeTracking {
    public interface IsChangeTracker {
        ChangeSet GetChanges();
        void Add(object item, EntityStatusTypes status = EntityStatusTypes.Unchanged);
    }

    public static class IsChangeTrackerExtensions {
        public static bool HasChanges(this IsChangeTracker changetracker) {
            return changetracker.GetChanges().Entries.Any();
        }
    }
}