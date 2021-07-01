using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace ExoActive
{
    public static partial class Type<TKey, TValue>
    {
        [DataContract]
        public abstract partial class Entity
        {
            [DataMember] public readonly Guid guid = Guid.NewGuid();
            [DataMember] protected readonly Dictionary<string, EntityStateMachine> states = new();
            [DataMember] protected readonly AttributeGroup attributes = new();
            [DataMember] protected readonly Traits traits = new();

            public AttributeGroup Attributes => attributes;
            public Traits Traits => traits;

            public bool IsPermittedTrigger<TStateMachine>(Enum trigger, CapabilityProcessData data)
                where TStateMachine : EntityStateMachine, new()
            {
                return GetState<TStateMachine>().GetPermittedTriggers(data).Contains(trigger);
            }

            public void AddState(EntityStateMachine state)
            {
                states.Add(state.Id, state);
            }

            public TStateMachine GetState<TStateMachine>() where TStateMachine : EntityStateMachine, new()
            {
                var stateId = StateHelper<TStateMachine>.Id;
                if (!states.TryGetValue(stateId, out var state))
                {
                    state = StateHelper<TStateMachine>.CreateState();
                    AddState(state);
                }

                return (TStateMachine) state;
            }

            public bool HasState<TStateMachine>() where TStateMachine : EntityStateMachine, new()
            {
                return states.ContainsKey(StateHelper<TStateMachine>.Id);
            }
        }
    }
}