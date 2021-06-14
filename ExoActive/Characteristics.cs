using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;


namespace ExoActive
{
    [DataContract]
    public class Characteristics
    {
        [DataMember]
        private readonly Dictionary<string, ulong> characteristicDict = new Dictionary<string, ulong>();

        private static (string, ulong) ParseEnum(Enum e)
        {
            string key = Key(e.GetType());
            ulong value = Convert.ToUInt64(e);
            return (key, value);
        }
        
        public void Add(Enum characteristic)
        {
            var (key, value) = ParseEnum(characteristic);

            if (characteristicDict.ContainsKey(key))
            {
                characteristicDict[key] |= value;
            }
            else
            {
                characteristicDict.Add(key, value);
            }
        }
        
        public void Remove(Enum characteristic)
        {
            var (key, value) = ParseEnum(characteristic);

            if (characteristicDict.ContainsKey(key))
            {
                characteristicDict[key] &= ~value;
            }
        }

        public bool Has(Enum characteristic)
        {
            var (key, value) = ParseEnum(characteristic);
            if (!characteristicDict.ContainsKey(key)) return false;
            
            return (characteristicDict[key] & value) == value;
        }

        private static string Key(Type E) => E.AssemblyQualifiedName ?? throw new InvalidOperationException();
        private static string Key<E>() => Key(typeof(E));
        public ulong Value<E>() => characteristicDict.GetValueOrDefault(Key<E>(), 0UL);
        public ulong Value(Type E) => characteristicDict.GetValueOrDefault(Key(E), 0UL);

        public override string ToString()
        {
            return characteristicDict.Aggregate("", (agg, kvp) => 
                agg + 
                (agg.Length > 0 ? ", " : "") + 
                $"{{{kvp.Key}, {Convert.ToString((long)kvp.Value, 2)}}}");
        }

        private sealed class DefaultEqualityComparer : IEqualityComparer<Characteristics>
        {
            public bool Equals(Characteristics x, Characteristics y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return x.characteristicDict.SequenceEqual(y.characteristicDict);
            }

            public int GetHashCode(Characteristics obj)
            {
                return (obj.characteristicDict != null ? obj.characteristicDict.GetHashCode() : 0);
            }
        }

        public static IEqualityComparer<Characteristics> DefaultComparer { get; } = new DefaultEqualityComparer();
    }
}