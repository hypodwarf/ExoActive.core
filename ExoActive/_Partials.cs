using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CSharp.RuntimeBinder;

namespace ExoActive
{
    public static partial class Type<TKey, TValue>
    {
        public readonly partial struct Attribute : IEquatable<Attribute>
        {
            public bool Equals(Attribute other)
            {
                return namedValue.Equals(other.namedValue)
                       && modifiers.SequenceEqual(other.modifiers)
                       && modifiedValue.Equals(other.modifiedValue)
                       && guid.Equals(other.guid)
                       && version == other.version;
            }

            public override bool Equals(object obj)
            {
                return obj is Attribute other && Equals(other);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(namedValue, modifiers, modifiedValue, guid, version);
            }

            public static bool operator ==(Attribute left, Attribute right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(Attribute left, Attribute right)
            {
                return !left.Equals(right);
            }
        }

        public readonly partial struct NamedValue : IEquatable<NamedValue>
        {
            public static NamedValue operator +(NamedValue lv, TValue rv)
            {
                TValue value;
                try
                {
                    value = (dynamic) lv.value + rv;
                }
                catch (RuntimeBinderException e)
                {
                    Console.Error.WriteLine(e.ToString());
                    // The values cannot be added, so the value from lv is used.
                    value = lv.value;
                }
                
                return new NamedValue(lv.name, value);
            }
            public static NamedValue operator +(NamedValue lv, NamedValue rv)
            {
                TValue value;
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

                return new NamedValue($"{lv.name} + {rv.name}", value);
            }

            public static NamedValue operator -(NamedValue lv, NamedValue rv)
            {
                TValue value;
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

                return new NamedValue($"{lv.name} - {rv.name}", value);
            }

            public static NamedValue operator *(NamedValue lv, NamedValue rv)
            {
                TValue value;
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

                return new NamedValue($"{lv.name} * {rv.name}", value);
            }

            public static NamedValue operator /(NamedValue lv, NamedValue rv)
            {
                TValue value;
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

                return new NamedValue($"{lv.name} / {rv.name}", value);
            }

            public bool Equals(NamedValue other)
            {
                return name == other.name && EqualityComparer<TValue>.Default.Equals(value, other.value);
            }

            public override bool Equals(object obj)
            {
                return obj is NamedValue other && Equals(other);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(name, value);
            }

            public static bool operator ==(NamedValue left, NamedValue right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(NamedValue left, NamedValue right)
            {
                return !left.Equals(right);
            }
        }

        public partial class Attributes
        {
            protected bool Equals(Attributes other)
            {
                return DefaultComparer.Equals(this, other);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((Attributes) obj);
            }

            public override int GetHashCode()
            {
                return DefaultComparer.GetHashCode(this);
            }

            private sealed class DefaultEqualityComparer : IEqualityComparer<Attributes>
            {
                public bool Equals(Attributes x, Attributes y)
                {
                    if (ReferenceEquals(x, y)) return true;
                    if (ReferenceEquals(x, null)) return false;
                    if (ReferenceEquals(y, null)) return false;
                    if (x.GetType() != y.GetType()) return false;
                    return x.SequenceEqual(y);
                }

                public int GetHashCode(Attributes obj)
                {
                    return obj.GetHashCode();
                }
            }

            public static IEqualityComparer<Attributes> DefaultComparer { get; } =
                new DefaultEqualityComparer();
        }

        public abstract partial class Entity : IEquatable<Entity>
        {
            public bool Equals(Entity other)
            {
                return DefaultComparer.Equals(this, other);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((Entity) obj);
            }

            public override int GetHashCode()
            {
                return DefaultComparer.GetHashCode(this);
            }

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
                           && x.states.Values.SequenceEqual<EntityStateMachine>(y.states.Values,
                               EntityStateMachine.DefaultComparer)
                           && Attributes.DefaultComparer.Equals(x.attributes, y.attributes)
                           && Traits.DefaultComparer.Equals(x.traits, y.traits);
                }

                public int GetHashCode(Entity entity)
                {
                    return HashCode.Combine(entity.states, entity.attributes, entity.traits);
                }
            }

            public static IEqualityComparer<Entity> DefaultComparer { get; } =
                new DefaultEqualityComparer();
        }

        public abstract partial class EntityStateMachine
        {
            private sealed class DefaultEqualityComparer : IEqualityComparer<EntityStateMachine>
            {
                public bool Equals(EntityStateMachine x, EntityStateMachine y)
                {
                    if (ReferenceEquals(x, y)) return true;
                    if (ReferenceEquals(x, null)) return false;
                    if (ReferenceEquals(y, null)) return false;
                    if (x.GetType() != y.GetType()) return false;
                    return x.CurrentState.Equals(y.CurrentState)
                           && x.LastTransitionTick == y.LastTransitionTick
                           && x.Entities.SequenceEqual(y.Entities);
                }

                public int GetHashCode(EntityStateMachine obj)
                {
                    return HashCode.Combine(obj.CurrentState, obj.LastTransitionTick, obj.Entities);
                }
            }

            public static IEqualityComparer<EntityStateMachine> DefaultComparer { get; } =
                new DefaultEqualityComparer();
        }
    }
}