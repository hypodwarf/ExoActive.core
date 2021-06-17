using ExoActive;

namespace Example
{
    public class Actor : Entity
    {
        public Actor()
        {
            attributes.Add(PhysicalAttributes.Strength, 10);
            attributes.Add(PhysicalAttributes.Weight, 3);
            traits.Add(PhysicalTraits.CanCarry);
        }
    }
}