using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.CSharp.RuntimeBinder;

namespace ExoActive
{
    /**
     * The Attribute struct starts with a base value from a NamedValue object. Each Attribute is given a GUID upon
     * creation and also maintains a version that increments whenever a change is made to the Attribute. The attribute
     * value can then be altered by inserting, updating and removing other Attribute objects. However, inserting
     * multiple Attributes with the same GUID is not allowed. This creates a tree structure that represents the
     * temporary changes to the base value.
     */
    [DataContract]
    public readonly partial struct Attribute<T>
    {
        [DataMember] public readonly NamedValue<T> namedValue;
        [DataMember] private readonly Attribute<T>[] modifiers;
        [DataMember] public readonly NamedValue<T> modifiedValue;
        [DataMember] public readonly Guid guid;
        [DataMember] public readonly ulong version;

        public Attribute(string name, T baseValue) : this(new NamedValue<T>(name, baseValue))
        {
        }

        public Attribute(NamedValue<T> namedValue) : this(namedValue, Array.Empty<Attribute<T>>(),
            Guid.NewGuid(), 0)
        {
        }

        private Attribute(Attribute<T> attribute, Attribute<T>[] modifiers) : this(attribute.namedValue, modifiers,
            attribute.guid, attribute.version + 1)
        {
        }

        private Attribute(NamedValue<T> namedValue, Attribute<T>[] modifiers, Guid guid, ulong version)
        {
            this.namedValue = namedValue;
            this.modifiers = modifiers;

            modifiedValue = modifiers.Aggregate(namedValue, (value, attribute) => value + attribute.modifiedValue);
            this.guid = guid;
            this.version = version;
        }

        private int GetModifierIdx(Attribute<T> modifier)
        {
            var i = 0;
            for (; i < modifiers.Length; i++)
                if (modifiers[i].guid.Equals(modifier.guid))
                    return i;

            return -1;
        }

        private Attribute<T> InsertModifierAtEnd(Attribute<T> modifier)
        {
            // Do not allow self insertion
            // if (this.guid.Equals(modifier.guid)) return this;

            var updatedModifiers = new Attribute<T>[modifiers.Length + 1];
            modifiers.CopyTo(updatedModifiers, 0);
            updatedModifiers[^1] = modifier;
            return new Attribute<T>(this, updatedModifiers);
        }

        public Attribute<T> InsertModifier(Attribute<T> modifier)
        {
            var i = GetModifierIdx(modifier);
            return i < 0 ? InsertModifierAtEnd(modifier) : this;
        }

        private Attribute<T> UpdateModifierAtIdx(Attribute<T> modifier, int idx)
        {
            // Don't update if the modifier hasn't changed
            if (modifier.Equals(modifiers[idx])) return this;

            var updatedModifiers = new Attribute<T>[modifiers.Length];
            modifiers.CopyTo(updatedModifiers, 0);
            updatedModifiers[idx] = modifier;
            return new Attribute<T>(this, updatedModifiers);
        }

        public Attribute<T> UpdateModifier(Attribute<T> modifier)
        {
            var i = GetModifierIdx(modifier);
            return i < 0 ? this : UpdateModifierAtIdx(modifier, i);
        }

        public Attribute<T> UpsertModifier(Attribute<T> modifier)
        {
            var i = GetModifierIdx(modifier);
            return i < 0 ? InsertModifierAtEnd(modifier) : UpdateModifierAtIdx(modifier, i);
        }

        private Attribute<T> RemoveModifierAtIdx(int idx)
        {
            var updatedModifiers = new Attribute<T>[modifiers.Length - 1];
            modifiers[..idx].CopyTo(updatedModifiers, 0);
            modifiers[(idx + 1)..].CopyTo(updatedModifiers, idx);
            return new Attribute<T>(this, updatedModifiers);
        }

        public Attribute<T> RemoveModifier(Attribute<T> modifier)
        {
            var i = GetModifierIdx(modifier);
            return i < 0 ? this : RemoveModifierAtIdx(i);
        }

        public ReadOnlyCollection<Attribute<T>> Modifiers => new(modifiers);
    }

    public readonly struct AttributeHelper
    {
        public static void PrintAttributeTree<T>(Attribute<T> attribute, int depth = 0)
        {
            var indent = "";
            for (var i = 0; i < depth; i++) indent += " ";

            Console.WriteLine(
                $"{indent}{attribute.namedValue.name} : {attribute.namedValue.value} - {attribute.modifiedValue.value} {{{attribute.guid} : {attribute.version}}}");

            foreach (var child in attribute.Modifiers) PrintAttributeTree(child, depth + 1);
        }
    }

    public readonly partial struct NamedValue<T>
    {
        public readonly string name;
        public readonly T value;

        public NamedValue(string name, T value)
        {
            this.name = name;
            this.value = value;
        }
    }
}