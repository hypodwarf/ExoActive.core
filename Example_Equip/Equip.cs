using System;
using System.Linq;
using ExoActive;

namespace Example_Equip
{
    public static class Equip
    {
        public class EquipItem : Capability
        {
            public EquipItem() : base(
                new ICapabilityProcess[]
                {
                    CapabilityTriggerProcess<EquipmentState>.Get(EquipmentState.Trigger.Equip)
                },
                new ICapabilityProcess[]
                {
                    CapabilityTriggerProcess<ItemEquippedState>.Get(ItemEquippedState.Trigger.Equip)
                })
            {
            }
        }
        
        public class UnequipItem : Capability
        {
            public UnequipItem() : base(
                new ICapabilityProcess[]
                {
                    CapabilityTriggerProcess<EquipmentState>.Get(EquipmentState.Trigger.Unequip)
                },
                new ICapabilityProcess[]
                {
                    CapabilityTriggerProcess<ItemEquippedState>.Get(ItemEquippedState.Trigger.Unequip)
                })
            {
            }
        }

        public class EquipmentState : EntityStateMachine
        {
            public new enum State
            {
                Equipment
            }
            
            public enum Trigger
            {
                Equip,
                Unequip
            }

            public EquipmentState() : base(State.Equipment)
            {
                Configure(State.Equipment)
                    .OnEntryFrom(GetTrigger(Trigger.Equip), data =>
                    {
                        data.targets.ForEach(target =>
                        {
                            var attributes = (Attributes)target.Attributes.Clone();
                            attributes.Add(target.Traits.Value<EquipmentTraits>() | EquipmentTraits.Equip, -1);
                            Entities.Add(target, attributes);
                            Owner.Attributes.Apply(attributes);
                        });
                    })
                    .OnEntryFrom(GetTrigger(Trigger.Unequip), data =>
                    {
                        data.targets.ForEach(target =>
                        {
                            Owner.Attributes.Revert(Entities[target]);
                            Entities.Remove(target);
                        });
                    })
                    .PermitReentryIf(GetTrigger(Trigger.Equip), data =>
                    {
                        return data.targets.All(target =>
                            target.GetState<ItemEquippedState>().CurrentState.Equals(ItemEquippedState.State.NotEquipped)
                            && Owner.Attributes.GetAttributeValue(target.Traits.Value<EquipmentTraits>() | EquipmentTraits.Equip) > 0
                            );
                    })
                    .PermitReentryIf(GetTrigger(Trigger.Unequip), data =>
                    {
                        return data.targets.All(target => Entities.Contains(target));
                    });
            }
        }

        public class ItemEquippedState : EntityStateMachine
        {
            public new enum State
            {
                IsEquipped,
                NotEquipped
            }

            public enum Trigger
            {
                Equip,
                Unequip
            }
            
            public ItemEquippedState() : base(State.NotEquipped)
            {
                Configure(State.NotEquipped)
                    .OnEntryFrom(GetTrigger(Trigger.Unequip), data =>
                    {
                        data.targets.ForEach(equipper => Entities.Remove(equipper));
                    })
                    .Permit(Trigger.Equip, State.IsEquipped);
                
                Configure(State.IsEquipped)
                    .OnEntryFrom(GetTrigger(Trigger.Equip), data =>
                    {
                        data.targets.ForEach(equipper => Entities.Add(equipper));
                    })
                    .Permit(Trigger.Unequip, State.NotEquipped);
            }
        }
    }
}