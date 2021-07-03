using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ExoActive
{
    public static class Manager
    {
        private static Dictionary<Guid, Entity> entities =
            new Dictionary<Guid, Entity>();

        public static ReadOnlyDictionary<Guid, Entity> Entities
        {
            get => new ReadOnlyDictionary<Guid, Entity>(entities);
            set => entities = new Dictionary<Guid, Entity>(value);
        }

        public static E New<E>() where E : Entity, new()
        {
            E entity = new E();
            entities.Add(entity.Guid, entity);
            return entity;
        }

        public static IEntity Get(Guid guid) => guid.Equals(Guid.Empty) ? null : entities[guid];

        public class ManagedEntity : Entity
        {
            protected ManagedEntity()
            {
                Manager.entities.Add(this.Guid, this);
            }
        }
    }

    public class EntitySet : Dictionary<Guid, Attributes>
    {
        public bool Add(IEntity entity, params System.Enum[] types)
        {
            var attributes = new Attributes();
            foreach (var type in types)
            {
                attributes.Add(type);
            }

            return Add(entity, attributes);
        }

        public bool Add(IEntity entity, Attributes attributes)
        {
            // return TryAdd(entity?.Guid ?? Guid.Empty, attributes);
            try
            {
                base.Add(entity?.Guid ?? Guid.Empty, attributes);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Remove(IEntity entity) => Remove(entity?.Guid ?? Guid.Empty);

        public bool Contains(IEntity entity) => Keys.Contains(entity?.Guid ?? Guid.Empty);

        public Attributes this[IEntity entity]
        {
            get => base[entity?.Guid ?? Guid.Empty];
            set => base[entity?.Guid ?? Guid.Empty] = value;
        }

        public List<IEntity> List
        {
            get => this.Aggregate(new List<IEntity>(this.Count),
                (acc, kvp) =>
                {
                    acc.Add(Manager.Get(kvp.Key));
                    return acc;
                });
        }

        public override string ToString() =>
            this.Aggregate("", (s, guid) => $"{(s.Length > 0 ? $"{s}, " : s)}{guid.Key}");
    }
}