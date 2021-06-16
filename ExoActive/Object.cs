using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace ExoActive
{
    [DataContract]
    public abstract class Object
    {
        [DataMember] protected readonly Dictionary<string, State> states = new();
        [DataMember] protected readonly Attributes attributes = new();
        [DataMember] protected readonly Traits traits = new();

        public bool IsPermittedTrigger<S>(Enum trigger) where S : State, new()
        {
            return GetState<S>().PermittedTriggers.Contains(trigger);
        }

        public void AddState(State state)
        {
            states.Add(state.Id, state);
        }

        public State GetState<S>() where S : State, new()
        {
            var stateId = StateHelper<S>.Id;
            return states[stateId];
        }

        public bool HasState<S>() where S : State, new()
        {
            return states.ContainsKey(StateHelper<S>.Id);
        }

        public Attributes Attributes => attributes;

        public Traits Traits => traits;

        private sealed class DefaultEqualityComparer : IEqualityComparer<Object>
        {
            public bool Equals(Object x, Object y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return x.states.Keys.SequenceEqual(y.states.Keys)
                       && x.states.Values.SequenceEqual(y.states.Values, State.DefaultComparer)
                       && Attributes.DefaultComparer.Equals(x.attributes, y.attributes)
                       && Traits.DefaultComparer.Equals(x.traits, y.traits);
            }

            public int GetHashCode(Object obj)
            {
                return HashCode.Combine(obj.states, obj.attributes, obj.traits);
            }
        }

        public static IEqualityComparer<Object> DefaultComparer { get; } = new DefaultEqualityComparer();
    }
}