namespace DataLink.Core.ChangeTracking {
    public struct ChangedValue {
        public string Path { get; set; }
        public object Value { get; set; }
        public ChangeTypes Type { get; set; }

        public override string ToString() {
            return "(" + Type + ") " + Path;
        }
    }
}