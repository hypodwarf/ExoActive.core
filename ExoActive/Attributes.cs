using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace ExoActive
{
    [DataContract]
    public partial class Attributes : Dictionary<Enum, Attribute>, ICloneable
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

        public void Add(Enum type, long value = default, string name = null)
        {
            base.Add(type, new Attribute(name ?? type.ToString(), value));
        }

        public bool Apply(Enum type, long value = default, string name = null)
        {
            return Apply(type, new Attribute(name ?? type.ToString(), value));
        }

        public bool Apply(Enum type, Attribute attribute)
        {
            if (!ContainsKey(type)) return false;
            base[type] = base[type].UpsertModifier(attribute);
            return true;
        }

        public bool AdjustNamedModifier(Enum type, string name, long adjustment)
        {
            if (!ContainsKey(type)) return false;
            return Apply(type, base[type].GetModifierByName(name).AdjustBaseValue(adjustment));
        }

        public List<Enum> Apply(Attributes attributes)
        {
            return PerformActionOnGroup(attributes, (a, b) => a.UpsertModifier(b));
        }

        public bool Has(Enum type)
        {
            return ContainsKey(type);
        }

        public long GetAttributeValue(Enum type, long defaultValue = 0L)
        {
            return !TryGetValue(type, out var attribute) ? defaultValue : attribute.modifiedValue.value;
        }

        private List<Enum> PerformActionOnGroup(Attributes attributes, AttributeAction action)
        {
            var failed = new List<Enum>();
            foreach (var attribute in attributes)
                if (ContainsKey(attribute.Key))
                    base[attribute.Key] = action(base[attribute.Key], attribute.Value);
                else
                    failed.Add(attribute.Key);

            // Console.WriteLine($"Attributes action failed for {failed.Aggregate("", (a, i) => a + " " + i)}");
            return failed;
        }

        public List<Enum> Revert(Attributes attributes)
        {
            return PerformActionOnGroup(attributes, (a, b) => a.RemoveModifier(b));
        }

        public void Reset()
        {
            Reset(Keys.ToArray());
        }

        public void Reset(params Enum[] types)
        {
            foreach (var type in types)
            {
                if (ContainsKey(type))
                {
                    base[type] = base[type].Reset();
                }
            }
        }
    }
}