using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace ExoActive
{
    [DataContract]
    public abstract class Entity
    {
        [DataMember] public readonly Guid guid = Guid.NewGuid();
        [DataMember] protected readonly Dictionary<string, State> states = new();
        [DataMember] protected readonly Attributes attributes = new();
        [DataMember] protected readonly Traits traits = new();
        
        public Attributes Attributes => attributes;
        public Traits Traits => traits;

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
}