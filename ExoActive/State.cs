using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExoActive
{
    public abstract class StateMachine<S, T> where S : Enum where T : Enum
    {
        public S CurrentState { get; private set; }
        public abstract Array States { get; }
        public abstract Array Triggers { get; }
        
        protected Stateless.StateMachine<S, T> machine { get; }

        public override string ToString()
        {
            return Stateless.Graph.UmlDotGraph.Format(machine.GetInfo());
        }

        protected StateMachine(S initialState)
        {
            CurrentState = initialState;
            
            machine = new Stateless.StateMachine<S, T>(
                () => CurrentState,
                s => CurrentState = s);
        }
        
        public bool IsInState(S state) => machine.IsInState(state);
        public bool CanFire(T trigger) => machine.CanFire(trigger);
        public bool CanFire(T trigger, out ICollection<string> unmetGuards) => machine.CanFire(trigger, out unmetGuards);
        public IEnumerable<T> PermittedTriggers { get => machine.PermittedTriggers; }
        public IEnumerable<T> GetPermittedTriggers(params object[] args) => machine.GetPermittedTriggers(args);
        
        public Task FireAsync(T trigger) => machine.FireAsync(trigger);
        public Task FireAsync<A>(
            Stateless.StateMachine<S, T>.TriggerWithParameters<A> trigger, A a
            ) => machine.FireAsync(trigger, a);
        public Task FireAsync<A, B>(
            Stateless.StateMachine<S, T>.TriggerWithParameters<A, B> trigger, A a, B b
            ) => machine.FireAsync(trigger, a, b);
        public Task FireAsync<A, B, C>(
            Stateless.StateMachine<S, T>.TriggerWithParameters<A, B, C> trigger, A a, B b, C c
            ) => machine.FireAsync(trigger, a, b, c);
        public void Fire(T trigger) => machine.Fire(trigger);
        public void Fire(Stateless.StateMachine<S, T>.TriggerWithParameters trigger, params object[] args) => machine.Fire(trigger, args);
        public void Fire<A>(
            Stateless.StateMachine<S, T>.TriggerWithParameters<A> trigger, A a
            ) => machine.Fire(trigger, a);
        public void Fire<A, B>(
            Stateless.StateMachine<S, T>.TriggerWithParameters<A, B> trigger, A a, B b
            ) => machine.Fire(trigger, a, b);
        public void Fire<A, B, C>(
            Stateless.StateMachine<S, T>.TriggerWithParameters<A, B, C> trigger, A a, B b, C c
            ) => machine.Fire(trigger, a, b, c);
    }

    public abstract class State : StateMachine<Enum, Enum>
    {
        public string Name { get; }

        protected State(string Name, Enum initialState) : base(initialState)
        {
            this.Name = Name;
        }
    }
}