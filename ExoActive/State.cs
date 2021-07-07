using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ExoActive
{
    [DataContract]
    public abstract class EnumStateMachine : Stateless.StateMachine<Enum, Enum>
    {
        [DataContract]
        protected class StateReference
        {
            [DataMember] private Enum State;

            public Enum get()
            {
                return State;
            }

            public void set(Enum state)
            {
                // Console.WriteLine(state);
                State = state;
            }

            public StateReference(Enum initialState)
            {
                State = initialState;
            }
        }

        [DataMember] private StateReference stateRef;

        public Enum CurrentState => base.State;

        public delegate void Transitioned(Transition transInfo);

        public new event Transitioned OnTransitionCompleted;

        protected EnumStateMachine(StateReference stateRef) : base(stateRef.get, stateRef.set)
        {
            this.stateRef = stateRef;

            base.OnTransitionCompleted(t => OnTransitionCompleted?.Invoke(t));
        }
    }

    [DataContract]
    public abstract partial class EntityStateMachine : EnumStateMachine
    {
        private static readonly Dictionary<Enum, TriggerWithParameters<CapabilityProcessData>>
            Triggers = new();

        protected static TriggerWithParameters<CapabilityProcessData> GetTrigger(Enum trigger)
        {
            if (!Triggers.TryGetValue(trigger, out var dataTrigger))
            {
                dataTrigger = new TriggerWithParameters<CapabilityProcessData>(trigger);
                Triggers[trigger] = dataTrigger;
            }

            return dataTrigger;
        }

        public string Id => StateHelper.Id(this);

        [DataMember] private Guid owner;
        public IEntity Owner
        {
            get => Manager.Get(owner);
            set => owner = value.Guid;
        }
        
        
        [DataMember] public EntitySet Entities { get; private set; }
        
        [DataMember] public EntityWatch EntityWatch { get; private set; }

        [DataMember] public ulong LastTransitionTick { get; private set; }

        protected virtual void OnTickEvent()
        {
        }

        private void TransitionHandler(Transition transInfo)
        {
            LastTransitionTick = TimeTicker.Ticks;
        }

        protected EntityStateMachine(Enum initialState) : base(new StateReference(initialState))
        {
            OnTransitionCompleted += TransitionHandler;
            TimeTicker.TickEvent += OnTickEvent;
            Entities = new EntitySet();
            EntityWatch = new EntityWatch();
        }

        public void Fire(Enum trigger, CapabilityProcessData data)
        {
            base.Fire(GetTrigger(trigger), data);
        }

        public override string ToString()
        {
            return $"{base.ToString()}, LastTick = {LastTransitionTick}, EntitiesCount = {Entities.Count}";
        }
    }

    public static class StateHelper<TStateMachine> where TStateMachine : EnumStateMachine, new()
    {
        public static string Id => StateHelper.Id(typeof(TStateMachine));

        public static TStateMachine CreateState()
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

        public static string Id(EnumStateMachine state)
        {
            return Id(state.GetType());
        }
    }
}