using ExoActive;

namespace Example_GroupCombined
{
    public class Ring : Manager.ManagedEntity
    {
        public Ring()
        {
            attributes.Add(PhysicalAttributes.Weight, 1);
            attributes.Add( PhysicalAttributes.Strength, 2);
            traits.Add(EquipmentTraits.Ring);
        }
    }
    
    public class Belt : Manager.ManagedEntity
    {
        public Belt()
        {
            attributes.Add(PhysicalAttributes.Weight, 3);
            
            attributes.Add( PhysicalAttributes.Strength, 5);
            traits.Add(EquipmentTraits.Belt);
        }
    }
    
    public class Item : Manager.ManagedEntity
    {
        public Item()
        {
            attributes.Add(PhysicalAttributes.Weight, 57);
            traits.Add(LiftingTraits.Lifteable);
        }
    }
}