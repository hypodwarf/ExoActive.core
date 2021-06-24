using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Claims;

namespace ExoActive
{
    public class Attributes : AttributeGroup<Enum, int>
    {
    }

    [DataContract]
    public partial class AttributeGroup<S, T>
    {
        [DataMember] private Dictionary<S, Attribute<T>> attributes = new();

        private delegate Attribute<T> AttributeAction(Attribute<T> a, Attribute<T> b);

        public AttributeGroup<S, T> Subset(params S[] types)
        {
            AttributeGroup<S, T> subset = new AttributeGroup<S, T>();
            subset.attributes = (Dictionary<S, Attribute<T>>) attributes.Where(pair => types.Contains(pair.Key));
            return subset;
        }

        public void Add(S type, T value = default, string name = null)
        {
            attributes.Add(type, new Attribute<T>(name ?? type.ToString(), value));
        }

        public void Remove(S type)
        {
            attributes.Remove(type);
        }

        public bool Has(S type)
        {
            return attributes.ContainsKey(type);
        }

        public T GetAttributeValue(S type)
        {
            return attributes[type].modifiedValue.value;
        }

        private List<S> PerformActionOnGroup(AttributeGroup<S, T> attributeGroup, AttributeAction action)
        {
            var failed = new List<S>();
            foreach (var (type, attribute) in attributeGroup.attributes)
                if (attributes.ContainsKey(type))
                    attributes[type] = action(attributes[type], attribute);
                else
                    failed.Add(type);

            return failed;
        }

        public List<S> Apply(AttributeGroup<S, T> attributeGroup)
        {
            return PerformActionOnGroup(attributeGroup, (a, b) => a.UpsertModifier(b));
        }

        public List<S> Revert(AttributeGroup<S, T> attributeGroup)
        {
            return PerformActionOnGroup(attributeGroup, (a, b) => a.RemoveModifier(b));
        }
    }
}