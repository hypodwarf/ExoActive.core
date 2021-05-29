using System;
using System.Collections.Generic;
using System.Linq;
using Stateless;

namespace ExoActive
{
    public class Object
    {
        private SC glass = new SC();
        private AttributeGroup<Enum, int> attributes = new AttributeGroup<Enum, int>();

        // public IEnumerable<Enum> GetTriggers()
        // {
        //     return glass.PermittedTriggers;
        // }

        public Enum GetState()
        {
            return glass.CurrentState;
        }
    }
}