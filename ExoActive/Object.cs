using System;
using System.Collections.Generic;
using System.Linq;

namespace ExoActive
{
    public abstract class Object
    {
        protected readonly IDictionary<Enum, State> states = new Dictionary<Enum, State>();
        protected readonly Attributes attributes = new Attributes();
        protected readonly Characteristics characteristics = new Characteristics();

        public IEnumerable<Enum> PermittedTriggers()
        {
            return states.Values.Aggregate(new List<Enum>(), (triggers, state) =>
            {
                triggers.AddRange(state.PermittedTriggers);
                return triggers;
            });
        }

        public IEnumerable<Enum> PermittedTriggers(Enum stateId)
        {
            return states[stateId].PermittedTriggers;
        }

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
    }
}