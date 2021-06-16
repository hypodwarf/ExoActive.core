using System.Runtime.CompilerServices;
using ExoActive;

namespace Example.Capabilities
{
    public class HoldState : State
    {
        static HoldState()
        {
            AddActorTargetTrigggers<Trigger>();
        }

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
                .Permit(Trigger.PutDown, State.Empty);
        }
    }

    public class PickUp : Capability
    {
        private static readonly CapabilityAction<HoldState> ActorPickUpAction =
            CapabilityAction<HoldState>.CreateFireAction(HoldState.Trigger.PickUp);

        public PickUp() : base(new ICapabilityAction[] {ActorPickUpAction})
        {
        }
    }

    public class PutDown : Capability
    {
        private static readonly CapabilityAction<HoldState> ActorPutDownAction =
            CapabilityAction<HoldState>.CreateFireAction(HoldState.Trigger.PutDown);

        public PutDown() : base(new ICapabilityAction[] {ActorPutDownAction})
        {
        }
    }
}