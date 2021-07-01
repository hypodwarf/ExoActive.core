using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;

namespace ExoActive
{
    public static partial class Type<TKey, TValue>
    {
        /**
     * The Attribute struct starts with a base value from a NamedValue object. Each Attribute is given a GUID upon
     * creation and also maintains a version that increments whenever a change is made to the Attribute. The attribute
     * value can then be altered by inserting, updating and removing other Attribute objects. However, inserting
     * multiple Attributes with the same GUID is not allowed. This creates a tree structure that represents the
     * temporary changes to the base value.
     */
        [DataContract]
        public readonly partial struct Attribute
        {
            [DataMember] public readonly NamedValue namedValue;
            [DataMember] private readonly Attribute[] modifiers;
            [DataMember] public readonly NamedValue modifiedValue;
            [DataMember] public readonly Guid guid;
            [DataMember] public readonly ulong version;

            public Attribute(string name, TValue baseValue) : this(new NamedValue(name, baseValue))
            {
            }

            public Attribute(NamedValue namedValue) : this(namedValue, Array.Empty<Attribute>(),
                Guid.NewGuid(), 0)
            {
            }

            private Attribute(Attribute attribute, Attribute[] modifiers) : this(attribute.namedValue,
                modifiers,
                attribute.guid, attribute.version + 1)
            {
            }

            private Attribute(NamedValue namedValue, Attribute[] modifiers, Guid guid, ulong version)
            {
                this.namedValue = namedValue;
                this.modifiers = modifiers;

                modifiedValue = modifiers.Aggregate(namedValue, (value, attribute) => value + attribute.modifiedValue);
                this.guid = guid;
                this.version = version;
            }

            private int GetModifierIdx(Attribute modifier)
            {
                var i = 0;
                for (; i < modifiers.Length; i++)
                    if (modifiers[i].guid.Equals(modifier.guid))
                        return i;

                return -1;
            }

            private Attribute InsertModifierAtEnd(Attribute modifier)
            {
                // Do not allow self insertion
                // if (this.guid.Equals(modifier.guid)) return this;

                var updatedModifiers = new Attribute[modifiers.Length + 1];
                modifiers.CopyTo(updatedModifiers, 0);
                updatedModifiers[^1] = modifier;
                return new Attribute(this, updatedModifiers);
            }

            public Attribute InsertModifier(Attribute modifier)
            {
                var i = GetModifierIdx(modifier);
                return i < 0 ? InsertModifierAtEnd(modifier) : this;
            }

            private Attribute UpdateModifierAtIdx(Attribute modifier, int idx)
            {
                // Don't update if the modifier hasn't changed
                if (modifier.Equals(modifiers[idx])) return this;

                var updatedModifiers = new Attribute[modifiers.Length];
                modifiers.CopyTo(updatedModifiers, 0);
                updatedModifiers[idx] = modifier;
                return new Attribute(this, updatedModifiers);
            }

            public Attribute UpdateModifier(Attribute modifier)
            {
                var i = GetModifierIdx(modifier);
                return i < 0 ? this : UpdateModifierAtIdx(modifier, i);
            }

            public Attribute UpsertModifier(Attribute modifier)
            {
                var i = GetModifierIdx(modifier);
                return i < 0 ? InsertModifierAtEnd(modifier) : UpdateModifierAtIdx(modifier, i);
            }

            private Attribute RemoveModifierAtIdx(int idx)
            {
                var updatedModifiers = new Attribute[modifiers.Length - 1];
                modifiers[..idx].CopyTo(updatedModifiers, 0);
                modifiers[(idx + 1)..].CopyTo(updatedModifiers, idx);
                return new Attribute(this, updatedModifiers);
            }

            public Attribute RemoveModifier(Attribute modifier)
            {
                var i = GetModifierIdx(modifier);
                return i < 0 ? this : RemoveModifierAtIdx(i);
            }

            public Attribute Reset() => new(this, Array.Empty<Attribute>());

            public ReadOnlyCollection<Attribute> Modifiers => new(modifiers);
        }

        public readonly struct AttributeHelper
        {
            public static void PrintAttributeTree(Attribute attribute, int depth = 0)
            {
                var indent = "";
                for (var i = 0; i < depth; i++) indent += " ";

                Console.WriteLine(
                    $"{indent}{attribute.namedValue.name} : {attribute.namedValue.value} - {attribute.modifiedValue.value} {{{attribute.guid} : {attribute.version}}}");

                foreach (var child in attribute.Modifiers) PrintAttributeTree(child, depth + 1);
            }
        }

        public readonly partial struct NamedValue
        {
            public readonly string name;
            public readonly TValue value;

            public NamedValue(string name, TValue value)
            {
                this.name = name;
                this.value = value;
            }
        }
    }
}