using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Stateless;
using Stateless.Graph;

namespace ExoActive
{
    public abstract class StateContainer<S, T> where S : struct, Enum where T : struct, Enum
    {
        public S CurrentState { get; private set; }
        public S[] States { get => Enum.GetValues<S>(); }
        public T[] Triggers { get => Enum.GetValues<T>(); }
        
        protected StateMachine<S, T> machine { get; }

        public override string ToString()
        {
            return UmlDotGraph.Format(machine.GetInfo());
        }

        public StateContainer(S initialState)
        {
            CurrentState = initialState;
            
            machine = new StateMachine<S, T>(
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
            StateMachine<S, T>.TriggerWithParameters<A> trigger, A a
            ) => machine.FireAsync(trigger, a);
        public Task FireAsync<A, B>(
            StateMachine<S, T>.TriggerWithParameters<A, B> trigger, A a, B b
            ) => machine.FireAsync(trigger, a, b);
        public Task FireAsync<A, B, C>(
            StateMachine<S, T>.TriggerWithParameters<A, B, C> trigger, A a, B b, C c
            ) => machine.FireAsync(trigger, a, b, c);
        public void Fire(T trigger) => machine.Fire(trigger);
        public void Fire(StateMachine<S, T>.TriggerWithParameters trigger, params object[] args) => machine.Fire(trigger, args);
        public void Fire<A>(
            StateMachine<S, T>.TriggerWithParameters<A> trigger, A a
            ) => machine.Fire(trigger, a);
        public void Fire<A, B>(
            StateMachine<S, T>.TriggerWithParameters<A, B> trigger, A a, B b
            ) => machine.Fire(trigger, a, b);
        public void Fire<A, B, C>(
            StateMachine<S, T>.TriggerWithParameters<A, B, C> trigger, A a, B b, C c
            ) => machine.Fire(trigger, a, b, c);
    }
    
    public enum State
    {
        Empty,
        HalfFull,
        Full
    }

    public enum Trigger
    {
        Fill,
        Drink
    }

    public class SC : StateContainer<State, Trigger>
    {
        public SC(State? initialState = null) : base(initialState ?? State.Empty)
        {
            machine.Configure(State.Empty)
                .OnEntry(() => Console.WriteLine("Empty"))
                .Permit(Trigger.Fill, State.HalfFull);
            
            machine.Configure(State.HalfFull)
                .OnEntryFrom(Trigger.Fill, () => Console.WriteLine("Inc"))
                .OnEntryFrom(Trigger.Drink, () => Console.WriteLine("Dec"))
                .Permit(Trigger.Fill, State.Full)
                .Permit(Trigger.Drink, State.Empty);
            
            machine.Configure(State.Full)
                .OnEntry(() => Console.WriteLine("Full"))
                .Permit(Trigger.Drink, State.HalfFull);
        }
    }

}