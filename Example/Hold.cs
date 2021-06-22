using System;
using System.ComponentModel;
using System.Linq;
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
        
        private void AddHolder(CapabilityProcessData data)
        {
            data.actors.ForEach(actor => Entities.Add(actor));
        }

        private void RemoveHolder(CapabilityProcessData data)
        {
            data.actors.ForEach(actor => Entities.Remove(actor));
        }

        public HeldState() : base(State.NotHeld)
        {
            Configure(State.NotHeld)
                .OnEntryFrom(GetTrigger(Trigger.PutDown), RemoveHolder)
                .Permit(Trigger.PickUp, State.Held);

            Configure(State.Held)
                .OnEntryFrom(GetTrigger(Trigger.PickUp), AddHolder)
                .Permit(Trigger.PutDown, State.NotHeld);
        }
    }
    public class HoldState : State
    {
        private new enum State
        {
            Holding,
        }

        internal enum Trigger
        {
            PickUp,
            PutDown
        }

        private void AddTargets(CapabilityProcessData data)
        {
            data.targets.ForEach(target => Entities.Add(target));
        }

        private void RemoveTargets(CapabilityProcessData data)
        {
            data.targets.ForEach(target => Entities.Remove(target));
        }

        public HoldState() : base(State.Holding)
        {
            Configure(State.Holding)
                .OnEntryFrom(GetTrigger(Trigger.PickUp), AddTargets)
                .OnEntryFrom(GetTrigger(Trigger.PutDown), RemoveTargets)
                .PermitReentry(Trigger.PickUp)
                .PermitReentry(Trigger.PutDown);
        }
    }

    public class PickUp : Capability
    {
        private static bool WeightCheck(CapabilityProcessData data)
        {
            var currentWeight = data.subject.GetState<HoldState>().Entities.Aggregate(0, 
                (value, guid) => value + Manager.Get(guid).Attributes.GetAttributeValue(PhysicalAttributes.Weight));

            var totalWeight = data.targets.Aggregate(currentWeight, 
                (value, target) => value + target.Attributes.GetAttributeValue(PhysicalAttributes.Weight));

            var maxWeight = data.subject.Attributes.GetAttributeValue(PhysicalAttributes.Strength);

            return maxWeight >= totalWeight;
        }

        private static bool NotAlreadyHoldingCheck(CapabilityProcessData data)
        {
            return !data.targets.Any(data.subject.GetState<HoldState>().Entities.Contains);
        }
        
        public PickUp() : base(new ICapabilityProcess[]
        {
            CapabilityTriggerProcess<HoldState>.Get(HoldState.Trigger.PickUp, CapabilityProcessData.DataFilter.Actors),
            CapabilityTriggerProcess<HeldState>.Get(HeldState.Trigger.PickUp, CapabilityProcessData.DataFilter.Targets),
            DelegateCheckProcess.IsTrue(WeightCheck), 
            DelegateCheckProcess.IsTrue(NotAlreadyHoldingCheck), 
        })
        {
        }
    }

    public class PutDown : Capability
    {
        private static bool IsHoldingCheck(CapabilityProcessData data)
        {
            return data.targets.All(data.subject.GetState<HoldState>().Entities.Contains);
        }
        
        public PutDown() : base(new ICapabilityProcess[]
        {
            CapabilityTriggerProcess<HoldState>.Get(HoldState.Trigger.PutDown, CapabilityProcessData.DataFilter.Actors),
            CapabilityTriggerProcess<HeldState>.Get(HeldState.Trigger.PutDown, CapabilityProcessData.DataFilter.Targets),
            DelegateCheckProcess.IsTrue(IsHoldingCheck), 
        })
        {
        }
    }
}