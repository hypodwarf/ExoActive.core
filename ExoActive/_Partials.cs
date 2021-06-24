using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CSharp.RuntimeBinder;

namespace ExoActive
{
    public readonly partial struct Attribute<T> : IEquatable<Attribute<T>>
    {
        public bool Equals(Attribute<T> other)
        {
            return namedValue.Equals(other.namedValue)
                   && modifiers.SequenceEqual(other.modifiers)
                   && modifiedValue.Equals(other.modifiedValue)
                   && guid.Equals(other.guid)
                   && version == other.version;
        }

        public override bool Equals(object obj)
        {
            return obj is Attribute<T> other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(namedValue, modifiers, modifiedValue, guid, version);
        }

        public static bool operator ==(Attribute<T> left, Attribute<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Attribute<T> left, Attribute<T> right)
        {
            return !left.Equals(right);
        }
    }

    public readonly partial struct NamedValue<T> : IEquatable<NamedValue<T>>
    {
           public static NamedValue<T> operator +(NamedValue<T> lv, NamedValue<T> rv)
        {
            T value;
            try
            {
                value = (dynamic) lv.value + rv.value;
            }
            catch (RuntimeBinderException e)
            {
                Console.Error.WriteLine(e.ToString());
                // The values cannot be added, so the value from lv is used.
                value = lv.value;
            }

            return new NamedValue<T>($"{lv.name} + {rv.name}", value);
        }

        public static NamedValue<T> operator -(NamedValue<T> lv, NamedValue<T> rv)
        {
            T value;
            try
            {
                value = (dynamic) lv.value - rv.value;
            }
            catch (RuntimeBinderException e)
            {
                Console.Error.WriteLine(e.ToString());
                // The values cannot be subtracted, so the value from lv is used.
                value = lv.value;
            }

            return new NamedValue<T>($"{lv.name} - {rv.name}", value);
        }

        public static NamedValue<T> operator *(NamedValue<T> lv, NamedValue<T> rv)
        {
            T value;
            try
            {
                value = (dynamic) lv.value * rv.value;
            }
            catch (RuntimeBinderException e)
            {
                Console.Error.WriteLine(e.ToString());
                // The values cannot be mulitplied, so the value from lv is used.
                value = lv.value;
            }

            return new NamedValue<T>($"{lv.name} * {rv.name}", value);
        }

        public static NamedValue<T> operator /(NamedValue<T> lv, NamedValue<T> rv)
        {
            T value;
            try
            {
                value = (dynamic) lv.value / rv.value;
            }
            catch (RuntimeBinderException e)
            {
                Console.Error.WriteLine(e.ToString());
                // The values cannot be divided, so the value from lv is used.
                value = lv.value;
            }

            return new NamedValue<T>($"{lv.name} / {rv.name}", value);
        }

        public bool Equals(NamedValue<T> other)
        {
            return name == other.name && EqualityComparer<T>.Default.Equals(value, other.value);
        }

        public override bool Equals(object obj)
        {
            return obj is NamedValue<T> other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(name, value);
        }

        public static bool operator ==(NamedValue<T> left, NamedValue<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(NamedValue<T> left, NamedValue<T> right)
        {
            return !left.Equals(right);
        }
    }

    public partial class AttributeGroup<S, T>
    {
        protected bool Equals(AttributeGroup<S, T> other)
        {
            return DefaultComparer.Equals(this, other);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AttributeGroup<S, T>) obj);
        }

        public override int GetHashCode()
        {
            return DefaultComparer.GetHashCode(this);
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

    public abstract partial class Entity
    {
        private sealed class DefaultEqualityComparer : IEqualityComparer<Entity>
        {
            public bool Equals(Entity x, Entity y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return x.guid.Equals(y.guid)
                       && x.states.Keys.SequenceEqual(y.states.Keys)
                       && x.states.Values.SequenceEqual(y.states.Values, State.DefaultComparer)
                       && Attributes.DefaultComparer.Equals(x.attributes, y.attributes)
                       && Traits.DefaultComparer.Equals(x.traits, y.traits);
            }

            public int GetHashCode(Entity entity)
            {
                return HashCode.Combine(entity.states, entity.attributes, entity.traits);
            }
        }

        public static IEqualityComparer<Entity> DefaultComparer { get; } = new DefaultEqualityComparer();
    }

    public abstract partial class State
    {
        private sealed class DefaultEqualityComparer : IEqualityComparer<State>
        {
            public bool Equals(State x, State y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return x.CurrentState.Equals(y.CurrentState) 
                       && x.LastTransitionTick == y.LastTransitionTick
                       && x.Entities.SequenceEqual(y.Entities);
            }

            public int GetHashCode(State obj)
            {
                return HashCode.Combine(obj.CurrentState, obj.LastTransitionTick, obj.Entities);
            }
        }

        public static IEqualityComparer<State> DefaultComparer { get; } = new DefaultEqualityComparer();
    }
}