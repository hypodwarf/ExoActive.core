using System;
using System.Collections.Generic;
using System.Linq;
using ExoActive;

namespace Example_GroupCombined
{
    public static class Lift
    {
        /**
     * This example demostrates how the each of the different peices of the system can be use to create an environment
     * where different Entities can interact.
     * The environment is one where some entities (actors) can list other entities (items). The rules are:
     * Each actor has an amount of strength that it can use to lift things (including itself).
     * Nope: Each actor has a number of limbs that can be used to lift items. Each limb can apply a max of 1/(number of arms) the total strength.
     * Nope: Each item has a max number of places that limbs can grab onto.
     * Both actors and items have a weight.
     * Strength and weight have the same units... 1 strength can lift 1 weight.
     *
     * This demonstrates the transfer or consumption of an attribute... strength is used to reduce weight. 
     */
        
        internal static void DistributeTargetWeight(IEntity target)
        {
            // Assumptions:
            // There is one and only one target.
            // New actors are not currently acting on the target.

            // Get the full set of actors (not just the ones triggering the action, but also
            // those that are currently lifting the target). The new actors are added/removed to the Entities list
            // during the state change, which occurs before this.
            var currentActors = target.GetState<LiftedState>().Entities.List;

            // Reset strength usage so we can reassign it evenly
            currentActors.ForEach(actor =>
            {
                actor.Attributes.Revert(actor.GetState<LiftingState>().Entities[target]);
                target.Attributes.Revert(target.GetState<LiftedState>().Entities[actor]);

                actor.GetState<LiftingState>().Entities[target].Reset();
                target.GetState<LiftedState>().Entities[actor].Reset();
            });

            // The weight of the target needs to be divided amoung the actors.
            // We start by simply dividing the weight evenly. We apply the weight to each actor, up to their max, and 
            // keep track of the extra amout. We then repeat the process for the actors that aren't maxed out until
            // all the weight is distributed orall the actors are maxed out.
            long remainingWeight;

            var actorQueue = new Queue<IEntity>(currentActors.Where(actor =>
                actor.Attributes.GetAttributeValue(PhysicalAttributes.Strength) > 0));

            while ((remainingWeight = target.Attributes.GetAttributeValue(PhysicalAttributes.Weight)) > 0 &&
                   actorQueue.Count > 0)
            {
                var dividedWeight = remainingWeight > actorQueue.Count ? remainingWeight / actorQueue.Count : 1;
                var actor = actorQueue.Dequeue();
                var actorCurrentStrength = actor.Attributes.GetAttributeValue(PhysicalAttributes.Strength);
                var carryWeight = actorCurrentStrength < dividedWeight ? actorCurrentStrength : dividedWeight;

                var strengthAttr = actor.GetState<LiftingState>().Entities[target];
                strengthAttr.Apply(PhysicalAttributes.Strength, -carryWeight);
                actor.Attributes.Apply(strengthAttr);

                var weightAttr = target.GetState<LiftedState>().Entities[actor];
                weightAttr.Apply(PhysicalAttributes.Weight, -carryWeight);
                target.Attributes.Apply(weightAttr);

                //Refresh Queue
                if (actorQueue.Count == 0)
                {
                    actorQueue = new Queue<IEntity>(currentActors.Where(entity =>
                        entity.Attributes.GetAttributeValue(PhysicalAttributes.Strength) > 0));
                }
            }
        }

        /** The state of an IEntity that can be lifted off the ground **/
        public class LiftedState : EntityStateMachine
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

            protected override void OnTickEvent()
            {
                var list = EntityWatch.UpdateAll();
                list.ForEach(Console.WriteLine);
                DistributeTargetWeight(Manager.Get(Owner));
            }

            private void AddHolder(CapabilityProcessData data)
            {
                data.actors.ForEach(actor => Entities.Add(actor, PhysicalAttributes.Weight));
                data.actors.ForEach(actor => EntityWatch.Add(actor, PhysicalAttributes.Strength));
            }

            private void RemoveHolder(CapabilityProcessData data)
            {
                data.actors.ForEach(actor =>
                {
                    data.subject.Attributes.Revert(Entities[actor]);
                    Entities.Remove(actor);
                    EntityWatch.Remove(actor);
                });
            }

            private Enum DynamicPickUp(CapabilityProcessData data)
            {
                var combinedStrength = data.actors.Aggregate(0L,
                    (acc, actor) => acc + actor.Attributes.GetAttributeValue(PhysicalAttributes.Strength));

                var remainingWeight = data.subject.Attributes.GetAttributeValue(PhysicalAttributes.Weight);

                return combinedStrength >= remainingWeight ? State.Aloft : State.Grappled;
            }

            private Enum DynamicPutdown(CapabilityProcessData data)
            {
                if (Entities.Count == data.actors.Count) return State.Released;

                var returnedWeight = Entities.Where(entity => data.actors.ConvertAll(e => e.Guid).Contains(entity.Key))
                    .Aggregate(0L,
                        (acc, kvp) => acc + kvp.Value.GetAttributeValue(PhysicalAttributes.Weight));

                var availableStrength = Entities.List.Where(entity => !data.actors.Contains(entity)).Aggregate(0L,
                    (acc, actor) => acc + actor.Attributes.GetAttributeValue(PhysicalAttributes.Strength));

                return availableStrength >= -returnedWeight ? State.Aloft : State.Grappled;
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
                    .PermitReentry(Trigger.PickUp);
            }
        }

        /** The state of an IEntity that can attempt to lift other items **/
        public class LiftingState : EntityStateMachine
        {
            public new enum State
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
                data.targets.ForEach(target => Entities.Add(target, PhysicalAttributes.Strength));
            }

            private void RemoveTargets(CapabilityProcessData data)
            {
                data.targets.ForEach(target =>
                {
                    data.subject.Attributes.Revert(Entities[target]);
                    Entities.Remove(target);
                });

            }

            public LiftingState() : base(State.Empty)
            {
                Configure(State.Empty)
                    .OnEntryFrom(GetTrigger(Trigger.PutDown), RemoveTargets)
                    .Permit(Trigger.PickUp, State.Lifting);

                Configure(State.Lifting)
                    .OnEntryFrom(GetTrigger(Trigger.PickUp), AddTargets)
                    .Permit(Trigger.PutDown, State.Empty);
            }
        }

        public class PickUp : Capability
        {
            // The actor cannot already be lifting an object
            private static bool NotAlreadyHolding(CapabilityProcessData data)
            {
                return !data.subject.GetState<LiftingState>().CurrentState.Equals(LiftingState.State.Lifting);
            }

            // Must be at least one target
            private static bool HasOneTarget(CapabilityProcessData data)
            {
                return data.targets.Count == 1;
            }

            public PickUp() : base(new ICapabilityProcess[]
            {
                CapabilityTriggerProcess<LiftingState>.Get(LiftingState.Trigger.PickUp),
                DelegateCheckProcess.IsTrue(NotAlreadyHolding),
                DelegateCheckProcess.IsTrue(HasOneTarget),
            }, new ICapabilityProcess[]
            {
                CapabilityTriggerProcess<LiftedState>.Get(LiftedState.Trigger.PickUp),
            })
            {
            }

            protected override void AfterAction(List<IEntity> actors, List<IEntity> targets)
            {
                Console.WriteLine($"{actors.Count} Actors PICKUP Target [{targets[0].Guid}]");
                targets.ForEach(DistributeTargetWeight);
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
                return data.targets.Count == 1;
            }

            public PutDown() : base(new ICapabilityProcess[]
            {
                CapabilityTriggerProcess<LiftingState>.Get(LiftingState.Trigger.PutDown),
                DelegateCheckProcess.IsTrue(IsHoldingCheck),
                DelegateCheckProcess.IsTrue(HasTargets),
            }, new ICapabilityProcess[]
            {
                CapabilityTriggerProcess<LiftedState>.Get(LiftedState.Trigger.PutDown),
            })
            {
            }
            
            protected override void AfterAction(List<IEntity> actors, List<IEntity> targets)
            {
                Console.WriteLine($"{actors.Count} Actors PUTDOWN Target [{targets[0].Guid}]");
                targets.ForEach(DistributeTargetWeight);
            }
        }
    }
}