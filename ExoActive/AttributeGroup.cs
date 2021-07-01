using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace ExoActive
{
    public static partial class Type<TKey, TValue>
    {
        [DataContract]
        public partial class AttributeGroup : Dictionary<TKey, Attribute>, ICloneable
        {
            private delegate Attribute AttributeAction(Attribute a, Attribute b);

            public AttributeGroup()
            {
            }

            protected AttributeGroup(AttributeGroup copy) : base(copy)
            {
            }

            public virtual object Clone()
            {
                return new AttributeGroup(this);
            }

            public void Add(TKey type, TValue value = default, string name = null)
            {
                base.Add(type, new Attribute(name ?? type.ToString(), value));
            }

            public bool Apply(TKey type, TValue value = default, string name = null)
            {
                if (!ContainsKey(type)) return false;
                base[type] = base[type].UpsertModifier(new Attribute(name ?? type.ToString(), value));
                return true;
            }

            public bool Has(TKey type)
            {
                return ContainsKey(type);
            }

            public TValue GetAttributeValue(TKey type)
            {
                return base[type].modifiedValue.value;
            }

            private List<TKey> PerformActionOnGroup(AttributeGroup attributeGroup, AttributeAction action)
            {
                var failed = new List<TKey>();
                foreach (var (type, attribute) in attributeGroup)
                    if (ContainsKey(type))
                        base[type] = action(base[type], attribute);
                    else
                        failed.Add(type);

                // Console.WriteLine($"Attributes action failed for {failed.Aggregate("", (a, i) => a + " " + i)}");
                return failed;
            }

            public List<TKey> Apply(AttributeGroup attributeGroup)
            {
                return PerformActionOnGroup(attributeGroup, (a, b) => a.UpsertModifier(b));
            }

            // public List<TKey> AddApply(AttributeGroup attributeGroup)
            // {
            //     foreach (var (type, attribute) in attributeGroup)
            //     {
            //         if (ContainsKey(type))
            //         {
            //             PerformActionOnGroup(attributeGroup, (a, b) => a.UpsertModifier(b));
            //         }
            //         else
            //         {
            //             Add(type, attribute);
            //         }
            //     }
            //
            //     return new List<TKey>();
            // }

            public List<TKey> Revert(AttributeGroup attributeGroup)
            {
                return PerformActionOnGroup(attributeGroup, (a, b) => a.RemoveModifier(b));
            }

            public void Reset()
            {
                Reset(Keys.ToArray());
            }

            public void Reset(params TKey[] types)
            {
                foreach (var type in types)
                {
                    base[type] = base[type].Reset();
                }
            }
        }
    }
}