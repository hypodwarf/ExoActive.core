using ExoActive;

namespace Example
{
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
                .PermitIf(GetTrigger(Trigger.PickUp), State.Full);

            Configure(State.Full)
                .PermitIf(GetTrigger(Trigger.PutDown), State.Empty);
        }
    }

    public class PickUp : Capability
    {
        public PickUp() : base(new ICapabilityProcess[] {CapabilityTriggerProcess<HoldState>.Get(HoldState.Trigger.PickUp)})
        {
        }
    }

    public class PutDown : Capability
    {
        public PutDown() : base(new ICapabilityProcess[] {CapabilityTriggerProcess<HoldState>.Get(HoldState.Trigger.PutDown)})
        {
        }
    }
}