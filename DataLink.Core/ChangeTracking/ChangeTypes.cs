using System;

namespace DataLink.Core.ChangeTracking {

    [Flags]
    public enum ChangeTypes {
        None = 0,
        Create = 1,
        Update = 2,
        Delete = 4,
        Save = Create | Update,
        Any = Create | Update | Delete
    }
}