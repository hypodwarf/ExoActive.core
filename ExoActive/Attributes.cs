using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace ExoActive
{
    public static partial class Type<TKey, TValue>
    {
        [DataContract]
        public partial class Attributes : Dictionary<TKey, Attribute>, ICloneable
        {
            private delegate Attribute AttributeAction(Attribute a, Attribute b);

            public Attributes()
            {
            }

            protected Attributes(Attributes copy) : base(copy)
            {
            }

            public virtual object Clone()
            {
                return new Attributes(this);
            }

            public void Add(TKey type, TValue value = default, string name = null)
            {
                base.Add(type, new Attribute(name ?? type.ToString(), value));
            }

            public bool Apply(TKey type, TValue value = default, string name = null)
            {
                return Apply(type, new Attribute(name ?? type.ToString(), value));
            }
            
            public bool Apply(TKey type, Attribute attribute)
            {
                if (!ContainsKey(type)) return false;
                base[type] = base[type].UpsertModifier(attribute);
                return true;
            }

            public bool AdjustNamedModifier(TKey type, string name, TValue adjustment)
            {
                return Apply(type, base[type].GetModifierByName(name).AdjustBaseValue(adjustment));
            }
            
            public List<TKey> Apply(Attributes attributes)
            {
                return PerformActionOnGroup(attributes, (a, b) => a.UpsertModifier(b));
            }
            
            public bool Has(TKey type)
            {
                return ContainsKey(type);
            }

            public TValue GetAttributeValue(TKey type)
            {
                return base[type].modifiedValue.value;
            }

            private List<TKey> PerformActionOnGroup(Attributes attributes, AttributeAction action)
            {
                var failed = new List<TKey>();
                foreach (var (type, attribute) in attributes)
                    if (ContainsKey(type))
                        base[type] = action(base[type], attribute);
                    else
                        failed.Add(type);

                // Console.WriteLine($"Attributes action failed for {failed.Aggregate("", (a, i) => a + " " + i)}");
                return failed;
            }

            // public List<TKey> AddApply(Attributes attributes)
            // {
            //     foreach (var (type, attribute) in attributes)
            //     {
            //         if (ContainsKey(type))
            //         {
            //             PerformActionOnGroup(attributes, (a, b) => a.UpsertModifier(b));
            //         }
            //         else
            //         {
            //             Add(type, attribute);
            //         }
            //     }
            //
            //     return new List<TKey>();
            // }

            public List<TKey> Revert(Attributes attributes)
            {
                return PerformActionOnGroup(attributes, (a, b) => a.RemoveModifier(b));
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