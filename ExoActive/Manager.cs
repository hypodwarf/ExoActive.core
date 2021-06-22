using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ExoActive
{
    public static class Manager
    {
        private static Dictionary<Guid, Entity> entities = new Dictionary<Guid, Entity>();
        
        public static ReadOnlyDictionary<Guid, Entity> Entities
        {
            get => new ReadOnlyDictionary<Guid, Entity>(entities);
            set => entities = new Dictionary<Guid, Entity>(value);
        }

        public static E New<E>() where E : Entity, new()
        {
            E entity = new E();
            entities.Add(entity.guid, entity);
            return entity;
        }

        public static Entity Get(Guid guid) => guid.Equals(Guid.Empty) ? null : entities[guid];
        
        public class ManagedEntity : ExoActive.Entity
        {
            protected ManagedEntity()
            {
                Manager.entities.Add(this.guid, this);
            }
        }
    }

    public class EntitySet : HashSet<Guid>
    {
        public bool Add(Entity entity) => Add(entity?.guid ?? Guid.Empty);

        public bool Remove(Entity entity) => Remove(entity?.guid ?? Guid.Empty);

        public bool Contains(Entity entity) => Contains(entity?.guid ?? Guid.Empty);

        public override string ToString() => this.Aggregate("", (s, guid) => $"{(s.Length>0 ? $"{s}, " : s)}{guid}");
    }
}