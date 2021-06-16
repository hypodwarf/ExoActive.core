using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ExoActive
{
    [DataContract]
    public abstract class StateMachine<S, T> : Stateless.StateMachine<S, T> where S : Enum where T : Enum
    {
        [DataContract]
        protected class StateReference
        {
            [DataMember]
            private S State;
            public S get() => State;
            public void set(S state)
            {
                Console.WriteLine(state);
                State = state;
            }

            public StateReference(S initialState)
            {
                State = initialState;
            }
        }

        [DataMember]
        private StateReference stateRef;

        public S CurrentState { 
            get => State;
        }
        public delegate void Transitioned(Transition transInfo);
        public new event Transitioned OnTransitionCompleted;

        protected StateMachine(StateReference stateRef) : base(stateRef.get, stateRef.set)
        {
            this.stateRef = stateRef;
            
            base.OnTransitionCompleted(t => OnTransitionCompleted?.Invoke(t));
        }
    }
    
    [DataContract]
    public abstract class State : StateMachine<Enum, Enum>
    {
        private static readonly Dictionary<Enum, TriggerWithParameters<Object[]>> ActorTargetTriggers =
            new Dictionary<Enum, TriggerWithParameters<Object[]>>();

        protected static TriggerWithParameters<Object[]> GetActorTargetTrigger(Enum trigger)
        {
            return ActorTargetTriggers[trigger];
        }
        
        protected static void AddActorTargetTrigggers<T>() where T : struct, Enum
        {
            foreach (var enumValue in Enum.GetValues<T>())
            {
                ActorTargetTriggers.Add(enumValue, new TriggerWithParameters<Object[]>(enumValue));
            }
        }
        
        public string Id
        {
            get => StateHelper.Id(this);
        }
        
        [DataMember]
        public ulong LastTransitionTick { get; private set; }

        protected virtual void OnTickEvent() {}

        private void TransitionHandler(Transition transInfo)
        {
            LastTransitionTick = TimeTicker.Ticks;
        }

        protected State(Enum initialState) : base(new StateReference(initialState))
        {
            OnTransitionCompleted += TransitionHandler;
            TimeTicker.TickEvent += OnTickEvent;
        }

        private sealed class DefaultEqualityComparer : IEqualityComparer<State>
        {
            public bool Equals(State x, State y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return x.CurrentState.Equals(y.CurrentState) && x.LastTransitionTick == y.LastTransitionTick;
            }

            public int GetHashCode(State obj)
            {
                return HashCode.Combine(obj.CurrentState, obj.LastTransitionTick);
            }
        }

        public static IEqualityComparer<State> DefaultComparer { get; } = new DefaultEqualityComparer();
    }

    public static class StateHelper<S> where S : State, new()
    {
        public static string Id { get => StateHelper.Id(typeof(S)); }
        public static S CreateState() => new S();
    }
    
    public static class StateHelper
    {
        internal static string Id (Type stateType) => stateType.AssemblyQualifiedName;
        public static string Id(State state) => Id(state.GetType());
    }

}