using ExoActive;

namespace Example_Equip
{
    public class Actor : Manager.ManagedEntity
    {
        public Actor()
        {
            attributes.Add(PhysicalAttributes.Strength, 10);
            attributes.Add(PhysicalAttributes.Weight, 3);
            attributes.Add(EquipmentTraits.Ring | EquipmentTraits.Equip, 2);
            attributes.Add(EquipmentTraits.Belt | EquipmentTraits.Equip, 1);
        }
    }
}