using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace ExoActive
{
    [DataContract]
    public abstract class Object
    {
        [DataMember]
        protected readonly Dictionary<string, State> states = new Dictionary<string, State>();
        [DataMember]
        protected readonly Attributes attributes = new Attributes();
        [DataMember]
        protected readonly Characteristics characteristics = new Characteristics();

        public bool IsPermittedTrigger(Enum trigger)
        {
            return states.Values.Any(state => state.PermittedTriggers.Contains(trigger));
        }

        public void AddState(State state)
        {
            states.Add(state.ID, state);
        }

        public State State(string stateId)
        {
            return states[stateId];
        }
        
        public bool HasState(string stateId)
        {
            return states.ContainsKey(stateId);
        }

        public Attributes Attributes
        {
            get => attributes;
        }

        public Characteristics Characteristics
        {
            get => characteristics;
        }

        private sealed class DefaultEqualityComparer : IEqualityComparer<Object>
        {
            public bool Equals(Object x, Object y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return x.states.Keys.SequenceEqual(y.states.Keys)
                       && x.states.Values.SequenceEqual(y.states.Values, ExoActive.State.DefaultComparer)
                       && Attributes.DefaultComparer.Equals(x.attributes, y.attributes) 
                       && Characteristics.DefaultComparer.Equals(x.characteristics, y.characteristics);
            }

            public int GetHashCode(Object obj)
            {
                return HashCode.Combine(obj.states, obj.attributes, obj.characteristics);
            }
        }

        public static IEqualityComparer<Object> DefaultComparer { get; } = new DefaultEqualityComparer();
    }
}