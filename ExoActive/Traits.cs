using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace ExoActive
{
    public partial class ExoActive<TKey, TValue>
    {
        [DataContract]
        public class Traits
        {
            [DataMember] private readonly Dictionary<string, ulong> traitDict = new();

            private static (string, ulong) ParseEnum(Enum e)
            {
                var key = Key(e.GetType());
                var value = Convert.ToUInt64(e);
                return (key, value);
            }

            public void Add(Enum trait)
            {
                var (key, value) = ParseEnum(trait);

                if (traitDict.ContainsKey(key))
                    traitDict[key] |= value;
                else
                    traitDict.Add(key, value);
            }

            public void Remove(Enum trait)
            {
                var (key, value) = ParseEnum(trait);

                if (traitDict.ContainsKey(key)) traitDict[key] &= ~value;
            }

            public bool Has(Enum trait)
            {
                var (key, value) = ParseEnum(trait);
                if (!traitDict.ContainsKey(key)) return false;

                return (traitDict[key] & value) == value;
            }

            private static string Key(Type E)
            {
                return E.AssemblyQualifiedName ?? throw new InvalidOperationException();
            }

            private static string Key<E>()
            {
                return Key(typeof(E));
            }

            public E Value<E>() where E : Enum
            {
                return (E) Enum.ToObject(typeof(E), traitDict.GetValueOrDefault(Key<E>(), 0UL));
            }

            public Enum Value(Type E)
            {
                return (Enum) Enum.ToObject(E, traitDict.GetValueOrDefault(Key(E), 0UL));
            }

            public override string ToString()
            {
                return traitDict.Aggregate("", (agg, kvp) =>
                    agg +
                    (agg.Length > 0 ? ", " : "") +
                    $"{{{kvp.Key}, {Convert.ToString((long) kvp.Value, 2)}}}");
            }

            private sealed class DefaultEqualityComparer : IEqualityComparer<Traits>
            {
                public bool Equals(Traits x, Traits y)
                {
                    if (ReferenceEquals(x, y)) return true;
                    if (ReferenceEquals(x, null)) return false;
                    if (ReferenceEquals(y, null)) return false;
                    if (x.GetType() != y.GetType()) return false;
                    return x.traitDict.SequenceEqual(y.traitDict);
                }

                public int GetHashCode(Traits obj)
                {
                    return obj.traitDict != null ? obj.traitDict.GetHashCode() : 0;
                }
            }

            public static IEqualityComparer<Traits> DefaultComparer { get; } = new DefaultEqualityComparer();
        }
    }
}