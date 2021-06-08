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
        public new event Transitioned OnTransitionCompleted;

        protected StateMachine(S initialState) : base(initialState)
        {
            base.OnTransitionCompleted(t => OnTransitionCompleted?.Invoke(t));
        }
    }

    public abstract class State : StateMachine<Enum, Enum>
    {
        public ulong lastTransitionTick { get; protected set; }

        protected virtual void OnTickEvent() {}

        private void TransitionHandler(Transition transInfo)
        {
            lastTransitionTick = TimeTicker.Ticks;
        }

        protected State(Enum initialState) : base(initialState)
        {
            OnTransitionCompleted += TransitionHandler;
            TimeTicker.TickEvent += OnTickEvent;
        }
    }
}