namespace DataLink.Core.ChangeTracking {
    public enum EntityStatusTypes {
        Unknown = 0,
        Detached,
        Unchanged,
        Added,
        Modified,
        Deleted
    }
}