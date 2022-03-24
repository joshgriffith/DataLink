using System;
using DataLink.Core.ChangeTracking.Graph;

namespace DataLink.Core.ChangeTracking {
    public class EntityState {
        public ObjectGraph Graph { get; set; }
        public EntityStatusTypes Status { get; set; }

        public EntityState(object entity, EntityStatusTypes status) {
            Graph = new ObjectGraph(entity);
            Status = status;
        }
    }
}