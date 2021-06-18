using System;
using ExoActive;

namespace Example
{
    public class HeldState : State
    {
        private new enum State
        {
            NotHeld,
            Held
        }

        internal enum Trigger
        {
            PickUp,
            PutDown
        }

        public HeldState() : base(State.NotHeld)
        {
            Configure(State.NotHeld)
                .Permit(Trigger.PickUp, State.Held);

            Configure(State.Held)
                .OnEntryFrom(GetTrigger(Trigger.PickUp), data => Console.WriteLine(data.subject))
                .Permit(Trigger.PutDown, State.NotHeld);
        }
    }
    public class HoldState : State
    {
        private new enum State
        {
            Empty,
            Full
        }

        internal enum Trigger
        {
            PickUp,
            PutDown
        }

        public HoldState() : base(State.Empty)
        {
            Configure(State.Empty)
                .Permit(Trigger.PickUp, State.Full);

            Configure(State.Full)
                .OnEntryFrom(GetTrigger(Trigger.PickUp), data => Console.WriteLine(data.subject))
                .Permit(Trigger.PutDown, State.Empty);
        }
    }

    public class PickUp : Capability
    {
        public PickUp() : base(new ICapabilityProcess[]
        {
            CapabilityTriggerProcess<HoldState>.Get(HoldState.Trigger.PickUp)
        })
        {
        }
    }

    public class PutDown : Capability
    {
        public PutDown() : base(new ICapabilityProcess[]
        {
            CapabilityTriggerProcess<HoldState>.Get(HoldState.Trigger.PutDown)
        })
        {
        }
    }
}