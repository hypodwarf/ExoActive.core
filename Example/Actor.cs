using ExoActive;

namespace Example
{
    public class Actor : Manager.ManagedEntity
    {
        public Actor()
        {
            attributes.Add(PhysicalAttributes.Strength, 10);
            attributes.Add(PhysicalAttributes.Weight, 3);
            traits.Add(PhysicalTraits.CanCarry);
        }
    }
}