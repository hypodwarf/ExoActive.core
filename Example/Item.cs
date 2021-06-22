using ExoActive;

namespace Example
{
    public class Item : Manager.ManagedEntity
    {
        public Item()
        {
            attributes.Add(PhysicalAttributes.Weight, 2);
            traits.Add(PhysicalTraits.Carriable);
        }
    }
}