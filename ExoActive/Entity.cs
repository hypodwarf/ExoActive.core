using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace ExoActive
{
    [DataContract]
    public abstract partial class Entity
    {
        [DataMember] public readonly Guid guid = Guid.NewGuid();
        [DataMember] protected readonly Dictionary<string, State> states = new();
        [DataMember] protected readonly Attributes attributes = new();
        [DataMember] protected readonly Traits traits = new();
        
        public Attributes Attributes => attributes;
        public Traits Traits => traits;

        public bool IsPermittedTrigger<S>(Enum trigger, CapabilityProcessData data) where S : State, new()
        {
            return GetState<S>().GetPermittedTriggers(data).Contains(trigger);
        }

        public void AddState(State state)
        {
            states.Add(state.Id, state);
        }

        public S GetState<S>() where S : State, new()
        {
            var stateId = StateHelper<S>.Id;
            return (S)states[stateId];
        }

        public bool HasState<S>() where S : State, new()
        {
            return states.ContainsKey(StateHelper<S>.Id);
        }
    }
}