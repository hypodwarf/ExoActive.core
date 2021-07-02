using static ExoActive.ExoActive<System.Enum, int>;

namespace Example_GroupLift
{
    public class Item : Manager.ManagedEntity
    {
        public Item()
        {
            attributes.Add(PhysicalAttributes.Weight, 2);
            attributes.Add(LiftingAttributes.Holds, 2);
            traits.Add(LiftingTraits.Lifteable);
        }
    }
}