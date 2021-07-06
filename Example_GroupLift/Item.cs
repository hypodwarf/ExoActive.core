using ExoActive;

namespace Example_GroupLift
{
    public class Item : Manager.ManagedEntity
    {
        public Item()
        {
            attributes.Add(PhysicalAttributes.Weight, 2);
            traits.Add(LiftingTraits.Lifteable);
        }
    }
}