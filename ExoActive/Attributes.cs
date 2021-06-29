using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace ExoActive
{
    public class Attributes : AttributeGroup<Enum, int>
    {
        public override object Clone()
        {
            return new Attributes(this);
        }

        public Attributes() { }

        protected Attributes(Attributes copy) : base(copy) { }
    }

    [DataContract]
    public partial class AttributeGroup<S, T> :  Dictionary<S, Attribute<T>>, ICloneable
    {
        private delegate Attribute<T> AttributeAction(Attribute<T> a, Attribute<T> b);

        public AttributeGroup() { }
        
        protected AttributeGroup(AttributeGroup<S, T> copy) : base(copy) { }

        public virtual object Clone()
        {
            return new AttributeGroup<S, T>(this);
        }

        public void Add(S type, T value = default, string name = null)
        {
            base.Add(type, new Attribute<T>(name ?? type.ToString(), value));
        }
        
        public bool Apply(S type, T value = default, string name = null)
        {
            if (!ContainsKey(type)) return false;
            base[type] = base[type].UpsertModifier(new Attribute<T>(name ?? type.ToString(), value));
            return true;
        }

        public bool Has(S type)
        {
            return ContainsKey(type);
        }

        public T GetAttributeValue(S type)
        {
            return base[type].modifiedValue.value;
        }

        private List<S> PerformActionOnGroup(AttributeGroup<S, T> attributeGroup, AttributeAction action)
        {
            var failed = new List<S>();
            foreach (var (type, attribute) in attributeGroup)
                if (ContainsKey(type))
                    base[type] = action(base[type], attribute);
                else
                    failed.Add(type);
            
            // Console.WriteLine($"Attributes action failed for {failed.Aggregate("", (a, i) => a + " " + i)}");
            return failed;
        }

        public List<S> Apply(AttributeGroup<S, T> attributeGroup)
        {
            return PerformActionOnGroup(attributeGroup, (a, b) => a.UpsertModifier(b));
        }

        // public List<S> AddApply(AttributeGroup<S, T> attributeGroup)
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
        //     return new List<S>();
        // }

        public List<S> Revert(AttributeGroup<S, T> attributeGroup)
        {
            return PerformActionOnGroup(attributeGroup, (a, b) => a.RemoveModifier(b));
        }

        public void Reset()
        {
            Reset(Keys.ToArray());
        }

        public void Reset(params S[] types)
        {
            foreach (var type in types)
            {
                base[type] = base[type].Reset();
            }
        }
    }
}