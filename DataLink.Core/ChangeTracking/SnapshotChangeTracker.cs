using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DataLink.Core.Interceptors;

namespace DataLink.Core.ChangeTracking {
    public class SnapshotChangeTracker : IsChangeTracker, IsLoadInterceptor {

        private readonly Dictionary<object, EntityState> _entities = new();
        private readonly ChangeSet _changeset = new();

        public ChangeSet GetChanges() {
            foreach (var entity in _entities.Values) {
                if (_changeset.Contains(entity.Graph.Value))
                    continue;

                if (entity.Status == EntityStatusTypes.Modified) {
                    _changeset.Add(entity.Graph.Value, new List<ChangedValue>(), EntityStatusTypes.Modified);
                    continue;
                }

                var changes = new List<ChangedValue>();
                changes.AddRange(entity.Graph.GetChanges());
                
                if (changes.Any())
                    _changeset.Add(entity.Graph.Value, changes, EntityStatusTypes.Modified);
            }

            return _changeset;
        }
        
        public void Add(object item, EntityStatusTypes status = EntityStatusTypes.Unchanged) {
            if(!_entities.ContainsKey(item))
                _entities.Add(item, new EntityState(item, status));
        }

        public void OnLoad(IEnumerable entities) {
            foreach (var entity in entities)
                Add(entity);
        }
    }
}