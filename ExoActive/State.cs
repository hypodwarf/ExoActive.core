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
            [DataMember] private S State;

            public S get()
            {
                return State;
            }

            public void set(S state)
            {
                // Console.WriteLine(state);
                State = state;
            }

            public StateReference(S initialState)
            {
                State = initialState;
            }
        }

        [DataMember] private StateReference stateRef;

        public S CurrentState => State;

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
        private static readonly Dictionary<Enum, TriggerWithParameters<CapabilityProcessData>> Triggers = new();

        protected static TriggerWithParameters<CapabilityProcessData> GetTrigger(Enum trigger)
        {
            if (!Triggers.TryGetValue(trigger, out var dataTrigger))
            {
                dataTrigger = new TriggerWithParameters<CapabilityProcessData>(trigger);
                Triggers[trigger] = dataTrigger;
            }
            return dataTrigger;
        }

        // protected static void AddActorTargetTrigggers<T>() where T : struct, Enum
        // {
        //     foreach (var enumValue in Enum.GetValues<T>())
        //         Triggers.Add(enumValue, new TriggerWithParameters<CapabilityProcessData>(enumValue));
        // }

        public string Id => StateHelper.Id(this);

        [DataMember] public ulong LastTransitionTick { get; private set; }

        protected virtual void OnTickEvent()
        {
        }

        private void TransitionHandler(Transition transInfo)
        {
            LastTransitionTick = TimeTicker.Ticks;
        }

        protected State(Enum initialState) : base(new StateReference(initialState))
        {
            OnTransitionCompleted += TransitionHandler;
            TimeTicker.TickEvent += OnTickEvent;
        }

        public void Fire(Enum trigger, CapabilityProcessData data)
        {
            base.Fire(GetTrigger(trigger), data);
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
        public static string Id => StateHelper.Id(typeof(S));

        public static S CreateState()
        {
            return new();
        }
    }

    public static class StateHelper
    {
        internal static string Id(Type stateType)
        {
            return stateType.AssemblyQualifiedName;
        }

        public static string Id(State state)
        {
            return Id(state.GetType());
        }
    }
}