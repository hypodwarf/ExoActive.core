using System;
using System.Collections.Generic;
using System.Linq;

namespace ExoActive
{
    public abstract class Object
    {
        // protected readonly IDictionary<Enum, Capability> capabilities = new Dictionary<Enum, Capability>();
        protected readonly IDictionary<Enum, State> states = new Dictionary<Enum, State>();
        protected readonly Attributes attributes = new Attributes();
        protected readonly Characteristics characteristics = new Characteristics();

        // protected IEnumerable<Enum> PermittedTriggers()
        // {
        //     return states.Values.Aggregate(new List<Enum>(), (triggers, state) =>
        //     {
        //         triggers.AddRange(state.PermittedTriggers);
        //         return triggers;
        //     });
        // }
        //
        // protected IEnumerable<Enum> PermittedTriggers(Enum stateId)
        // {
        //     return states[stateId].PermittedTriggers;
        // }

        public bool IsPermittedTrigger(Enum trigger)
        {
            return states.Values.Any(state => state.PermittedTriggers.Contains(trigger));
        }

        // public IEnumerable<Enum> Capabilities()
        // {
        //     return capabilities.Keys;
        // }
        //
        // public Capability Capabilities(Enum id)
        // {
        //     return capabilities[id];
        // }

        public State State(Enum stateId)
        {
            return states[stateId];
        }

        public Attributes Attributes
        {
            get => attributes;
        }

        public Characteristics Characteristics
        {
            get => characteristics;
        }

        // public bool PerformCapability(Enum capabilityId)
        // {
        //     return Capabilities(capabilityId).Perform(this);
        // }
    }
}