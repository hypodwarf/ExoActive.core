using System;
using System.Collections.Generic;
using System.Linq;
using Stateless;

namespace ExoActive
{
    public class Object
    {
        private List<State> states = new List<State>();
        private Attributes attributes = new Attributes();

        // public IEnumerable<Enum> GetTriggers()
        // {
        //     return glass.PermittedTriggers;
        // }
        //
        // public Enum GetState()
        // {
        //     return glass.CurrentState;
        // }
    }
}