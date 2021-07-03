using ExoActive;

namespace Example_Equip
{
    public class Ring : Manager.ManagedEntity
    {
        public Ring()
        {
            attributes.Add(PhysicalAttributes.Weight, 1);
            traits.Add(EquipmentTraits.Ring);
        }
    }
    
    public class Belt : Manager.ManagedEntity
    {
        public Belt()
        {
            attributes.Add(PhysicalAttributes.Weight, 3);
            traits.Add(EquipmentTraits.Belt);
        }
    }
}