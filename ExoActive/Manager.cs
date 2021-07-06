using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml;

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
    
    // Stores a list of attribute versions for an Entity it can be checked for changes
    public class EntityWatch : Dictionary<Guid, Dictionary<Enum, ulong>>
    {
        private static bool TryGetAttributeVersion(IEntity entity, Enum type, out ulong version)
        {
             version = entity.Attributes.Has(type) ? entity.Attributes[type].version : 0;
             return entity.Attributes.Has(type);
        }

        public List<Enum> UpdateAll()
        {
            return Keys.Aggregate(new HashSet<Enum>(), (set, guid) =>
            {
                IEntity entity = Manager.Get(guid);
                Update(entity).ForEach(type => set.Add(type));
                return set;
            }).ToList();
        }

        public Dictionary<IEntity, List<Enum>> UpdateAllReport()
        {
            return Keys.Aggregate(new Dictionary<IEntity, List<Enum>>(), (dictionary, guid) =>
            {
                IEntity entity = Manager.Get(guid);
                dictionary.Add(entity, Update(entity));
                return dictionary;
            });
        }

        public List<Enum> Update(IEntity entity)
        {
            if (!TryGetValue(entity.Guid, out var versionDict))
            {
                return new List<Enum>();
            }

            var attributeTypes = versionDict.Keys;
            var changed = versionDict.Keys.Where(type => entity.Attributes[type].version != versionDict[type]).ToList();
            
            versionDict.Clear();
            
            foreach (var type in attributeTypes)
            {
                if (TryGetAttributeVersion(entity, type, out var version))
                {
                    versionDict.Add(type, version);
                }
            }

            return changed;
        }
        
        public bool Add(IEntity entity, params Enum[] attributeTypes)
        {
            if(ContainsKey(entity.Guid)){
                return false;
            }
            
            var versionDict = new Dictionary<Enum, ulong>();
            
            Array.ForEach(attributeTypes, type =>
            {
                TryGetAttributeVersion(entity, type, out var version);
                versionDict.Add(type, version);
            });

            base.Add(entity.Guid, versionDict);

            return true;
        }

        public bool Remove(IEntity entity) => base.Remove(entity.Guid);
    }
    
    public class EntitySet : Dictionary<Guid, Attributes>
    {
        public bool Add(IEntity entity, params Enum[] types)
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