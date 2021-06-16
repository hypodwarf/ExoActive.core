using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace ExoActive
{
    public class Attributes : AttributeGroup<Enum, int>
    {
    }

    [DataContract]
    public class AttributeGroup<S, T>
    {
        [DataMember] private Dictionary<S, Attribute<T>> attributes = new();

        private delegate Attribute<T> AttributeAction(Attribute<T> a, Attribute<T> b);

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

        private sealed class DefaultEqualityComparer : IEqualityComparer<AttributeGroup<S, T>>
        {
            public bool Equals(AttributeGroup<S, T> x, AttributeGroup<S, T> y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return x.attributes.SequenceEqual(y.attributes);
            }

            public int GetHashCode(AttributeGroup<S, T> obj)
            {
                return obj.attributes != null ? obj.attributes.GetHashCode() : 0;
            }
        }

        public static IEqualityComparer<AttributeGroup<S, T>> DefaultComparer { get; } = new DefaultEqualityComparer();
    }
}