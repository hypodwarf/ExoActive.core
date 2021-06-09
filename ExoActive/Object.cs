using System;
using System.Collections.Generic;
using System.Linq;

namespace ExoActive
{
    public abstract class Object
    {
        protected readonly IDictionary<string, State> states = new Dictionary<string, State>();
        protected readonly Attributes attributes = new Attributes();
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
    }
}