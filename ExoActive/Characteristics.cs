using System;
using System.Collections.Generic;
using System.Linq;

namespace ExoActive
{
    public class Characteristics
    {
        private readonly Dictionary<string, ulong> characteristicDict = new Dictionary<string, ulong>();

        private static (string, ulong) ParseEnum(Enum e)
        {
            string key = e.GetType().FullName;
            if (key == null)
            {
                throw new InvalidOperationException();
            }

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

        public override string ToString()
        {
            return characteristicDict.Aggregate("", (agg, kvp) => agg += (agg.Length > 0 ? ", " : "") + $"{{{kvp.Key}, {Convert.ToString((long)kvp.Value, 2)}}}");
        }
    }
}