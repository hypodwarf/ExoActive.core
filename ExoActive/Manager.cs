using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ExoActive
{
    public static partial class Type<TKey, TValue>
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
                entities.Add(entity.guid, entity);
                return entity;
            }

            public static Entity Get(Guid guid) => guid.Equals(Guid.Empty) ? null : entities[guid];

            public class ManagedEntity : Entity
            {
                protected ManagedEntity()
                {
                    Manager.entities.Add(this.guid, this);
                }
            }
        }

        public class EntitySet : Dictionary<Guid, AttributeGroup>
        {
            public bool Add(Entity entity, params TKey[] types)
            {
                var attributes = new AttributeGroup();
                foreach (var type in types)
                {
                    attributes.Add(type);
                }

                return Add(entity, attributes);
            }

            public bool Add(Entity entity, AttributeGroup attributes)
            {
                return TryAdd(entity?.guid ?? Guid.Empty, attributes);
            }

            public bool Remove(Entity entity) => Remove(entity?.guid ?? Guid.Empty);

            public bool Contains(Entity entity) => Keys.Contains(entity?.guid ?? Guid.Empty);

            public AttributeGroup this[Entity entity]
            {
                get => base[entity?.guid ?? Guid.Empty];
                set => base[entity?.guid ?? Guid.Empty] = value;
            }

            public List<Entity> List
            {
                get => this.Aggregate(new List<Entity>(this.Count),
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
}