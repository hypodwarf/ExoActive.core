using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExoActive
{
    public abstract class StateMachine<S, T> : Stateless.StateMachine<S, T> where S : Enum where T : Enum
    {
        public S CurrentState { get => State;}
        public abstract Array States { get; }
        public abstract Array Triggers { get; }
        public delegate void Transitioned(Transition transInfo);
        public event Transitioned OnTransitionComplete; 
        public event Transitioned OnTransition; 

        public override string ToString()
        {
            return Stateless.Graph.UmlDotGraph.Format(GetInfo());
        }

        protected StateMachine(S initialState) : base(initialState)
        {
            OnTransitionCompleted(t => OnTransitionComplete?.Invoke(t));
            OnTransitioned(t => OnTransition?.Invoke(t));
        }
    }

    public abstract class State : StateMachine<Enum, Enum>
    { 
        protected State(Enum initialState) : base(initialState)
        {}
    }
}