using System;
using System.Linq;
using ExoActive;

namespace Example
{
    /**
     * This example demostrates how the each of the different peices of the system can be use to create an environment
     * where different Entities can interact.
     * The environment is one where some entities (actors) can list other entities (items). The rules are:
     * Each actor has an amount of strength that it can use to lift things (including itself).
     * Each actor has a number of limbs that can be used to lift items. Each limb can apply a max of 1/2 the total strength.
     * Each item has a max number of places that limbs can grab onto.
     * Both actors and items have a weight.
     * Strength and weight have the same units... 1 strength can lift 1 weight.
     */
    
    /** The state of an Entity that can be lifted off the ground **/
    public class LiftedState : State
    {
        private new enum State
        {
            Released,
            Grappled,
            Aloft
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

        private Enum DynamicPickUp(CapabilityProcessData data)
        {
            Console.WriteLine($"T-pickup:{data.subject.guid}");
            return State.Aloft;
        }
        
        private Enum DynamicPutdown(CapabilityProcessData data)
        {
            Console.WriteLine($"T-putdown:{data.subject.guid}");
            if(Entities.Count == data.actors.Count) return State.Released;
            
            return State.Grappled;
        }

        public LiftedState() : base(State.Released)
        {
            Configure(State.Released)
                .OnEntryFrom(GetTrigger(Trigger.PutDown), RemoveHolder)
                .PermitDynamic(GetTrigger(Trigger.PickUp), DynamicPickUp);

            Configure(State.Grappled)
                .OnEntryFrom(GetTrigger(Trigger.PutDown), RemoveHolder)
                .OnEntryFrom(GetTrigger(Trigger.PickUp), AddHolder)
                .PermitDynamic(GetTrigger(Trigger.PutDown), DynamicPutdown)
                .PermitDynamic(GetTrigger(Trigger.PickUp), DynamicPickUp);

            Configure(State.Aloft)
                .OnEntryFrom(GetTrigger(Trigger.PutDown), RemoveHolder)
                .OnEntryFrom(GetTrigger(Trigger.PickUp), AddHolder)
                .PermitDynamic(GetTrigger(Trigger.PutDown), DynamicPutdown)
                .PermitDynamic(GetTrigger(Trigger.PickUp), DynamicPickUp);
        }
    }
    
    /** The state of an Entity that can attempt to lift other items **/
    public class LiftingState : State
    {
        private new enum State
        {
            Empty,
            Lifting,
        }

        internal enum Trigger
        {
            PickUp,
            PutDown
        }

        private void AddTargets(CapabilityProcessData data)
        {
            Console.WriteLine($"A-add: {data.subject.guid}");
            data.targets.ForEach(target => Entities.Add(target));
        }

        private void RemoveTargets(CapabilityProcessData data)
        {
            Console.WriteLine($"A-remove: {data.subject.guid}");
            data.targets.ForEach(target => Entities.Remove(target));
        }

        private bool WillBeEmpty(CapabilityProcessData data)
        {
            return data.targets.Count == Entities.Count;
        }

        private bool WillNotBeEmpty(CapabilityProcessData data) => !WillBeEmpty(data);

        public LiftingState() : base(State.Empty)
        {
            Configure(State.Empty)
                .OnEntryFrom(GetTrigger(Trigger.PutDown), RemoveTargets)
                .Permit(Trigger.PickUp, State.Lifting);

            Configure(State.Lifting)
                .OnEntryFrom(GetTrigger(Trigger.PickUp), AddTargets)
                .OnEntryFrom(GetTrigger(Trigger.PutDown), RemoveTargets)
                .PermitReentry(Trigger.PickUp)
                .PermitReentryIf(GetTrigger(Trigger.PutDown), WillNotBeEmpty)
                .PermitIf(GetTrigger(Trigger.PutDown), State.Empty, WillBeEmpty);
        }
    }

    public class PickUp : Capability
    {
        private static bool WeightCheck(CapabilityProcessData data)
        {
            var currentWeight = data.subject.GetState<LiftingState>().Entities.Aggregate(0, 
                (value, entry) => value + Manager.Get(entry.Key).Attributes.GetAttributeValue(PhysicalAttributes.Weight));

            var totalWeight = data.targets.Aggregate(currentWeight, 
                (value, target) => value + target.Attributes.GetAttributeValue(PhysicalAttributes.Weight));

            var maxWeight = data.subject.Attributes.GetAttributeValue(PhysicalAttributes.Strength);

            return maxWeight >= totalWeight;
        }

        private static bool NotAlreadyHoldingCheck(CapabilityProcessData data)
        {
            return !data.targets.Any(data.subject.GetState<LiftingState>().Entities.Contains);
        }
        
        private static bool HasTargets(CapabilityProcessData data)
        {
            return data.targets.Count > 0;
        }
        
        public PickUp() : base(new ICapabilityProcess[] {
            CapabilityTriggerProcess<LiftingState>.Get(LiftingState.Trigger.PickUp),
            DelegateCheckProcess.IsTrue(WeightCheck), 
            DelegateCheckProcess.IsTrue(NotAlreadyHoldingCheck), 
            DelegateCheckProcess.IsTrue(HasTargets),
        },new ICapabilityProcess[] {
            CapabilityTriggerProcess<LiftedState>.Get(LiftedState.Trigger.PickUp),
        })
        {
        }
    }

    public class PutDown : Capability
    {
        private static bool IsHoldingCheck(CapabilityProcessData data)
        {
            return data.targets.All(data.subject.GetState<LiftingState>().Entities.Contains);
        }

        private static bool HasTargets(CapabilityProcessData data)
        {
            return data.targets.Count > 0;
        }
        
        public PutDown() : base(new ICapabilityProcess[] {
            CapabilityTriggerProcess<LiftingState>.Get(LiftingState.Trigger.PutDown),
            DelegateCheckProcess.IsTrue(IsHoldingCheck), 
            DelegateCheckProcess.IsTrue(HasTargets),
        }, new ICapabilityProcess[] {
            CapabilityTriggerProcess<LiftedState>.Get(LiftedState.Trigger.PutDown),
        })
        {
        }
    }
}