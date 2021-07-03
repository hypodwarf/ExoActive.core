using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace ExoActive
{
    [DataContract]
    public partial class Attributes : Dictionary<System.Enum, Attribute>, ICloneable
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

        public void Add(System.Enum type, long value = default, string name = null)
        {
            base.Add(type, new Attribute(name ?? type.ToString(), value));
        }

        public bool Apply(System.Enum type, long value = default, string name = null)
        {
            return Apply(type, new Attribute(name ?? type.ToString(), value));
        }

        public bool Apply(System.Enum type, Attribute attribute)
        {
            if (!ContainsKey(type)) return false;
            base[type] = base[type].UpsertModifier(attribute);
            return true;
        }

        public bool AdjustNamedModifier(System.Enum type, string name, long adjustment)
        {
            return Apply(type, base[type].GetModifierByName(name).AdjustBaseValue(adjustment));
        }

        public List<System.Enum> Apply(Attributes attributes)
        {
            return PerformActionOnGroup(attributes, (a, b) => a.UpsertModifier(b));
        }

        public bool Has(System.Enum type)
        {
            return ContainsKey(type);
        }

        public long GetAttributeValue(System.Enum type)
        {
            return base[type].modifiedValue.value;
        }

        private List<System.Enum> PerformActionOnGroup(Attributes attributes, AttributeAction action)
        {
            var failed = new List<System.Enum>();
            foreach (var attribute in attributes)
                if (ContainsKey(attribute.Key))
                    base[attribute.Key] = action(base[attribute.Key], attribute.Value);
                else
                    failed.Add(attribute.Key);

            // Console.WriteLine($"Attributes action failed for {failed.Aggregate("", (a, i) => a + " " + i)}");
            return failed;
        }

        // public List<System.Enum> AddApply(Attributes attributes)
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
        //     return new List<System.Enum>();
        // }

        public List<System.Enum> Revert(Attributes attributes)
        {
            return PerformActionOnGroup(attributes, (a, b) => a.RemoveModifier(b));
        }

        public void Reset()
        {
            Reset(Keys.ToArray());
        }

        public void Reset(params System.Enum[] types)
        {
            foreach (var type in types)
            {
                base[type] = base[type].Reset();
            }
        }
    }
}