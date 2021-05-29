
using System;
using System.Collections.Generic;

namespace ExoActive
{
    public class AttributeGroup<S, T>
    {
        private readonly Dictionary<S, Attribute<T>> attributes = new Dictionary<S, Attribute<T>>();

        private delegate Attribute<T> AttributeAction(Attribute<T> a, Attribute<T> b);

        public void AddAttribute(S type, T value = default, string name = null) =>
            attributes.Add(type, new Attribute<T>(name ?? type.ToString(), value));

        public void RemoveAttribute(S type) => 
            attributes.Remove(type);

        public bool HasAttribute(S type) => 
            attributes.ContainsKey(type);

        public T GetAttributeValue(S type)
        {
            return attributes[type].modifiedValue.value;
        }

        private List<S> PerformActionOnGroup(AttributeGroup<S, T> attributeGroup, AttributeAction action)
        {
            List<S> failed = new List<S>();
            foreach ((var type, var attribute) in attributeGroup.attributes)
            {
                if (attributes.ContainsKey(type))
                {
                    attributes[type] = action(attributes[type], attribute);
                }
                else
                {
                    failed.Add(type);
                }
            }

            return failed;
        }

        public List<S> ApplyModification(AttributeGroup<S, T> attributeGroup)
        {
            return PerformActionOnGroup(attributeGroup, (a, b) => a.UpsertModifier(b));
        }

        public List<S> RemoveModification(AttributeGroup<S, T> attributeGroup)
        {
            return PerformActionOnGroup(attributeGroup, (a, b) => a.RemoveModifier(b));
        }
    }
}