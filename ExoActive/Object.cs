using System;
using System.Collections.Generic;
using System.Linq;

namespace ExoActive
{
    public abstract class Object
    {
        protected readonly ISet<State> states = new HashSet<State>();
        protected readonly Attributes attributes = new Attributes();
        protected readonly Characteristics characteristics = new Characteristics();

        public List<Enum> PermittedTriggers
        {
            get => states.Aggregate(new List<Enum>(), (triggers, state) =>
            {
                triggers.AddRange(state.PermittedTriggers);
                return triggers;
            });
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