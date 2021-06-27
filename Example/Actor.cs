using System;
using ExoActive;

namespace Example
{
    public class Actor : Manager.ManagedEntity
    {
        public Actor()
        {
            attributes.Add(PhysicalAttributes.Strength, new Random().Next(2,10));
            attributes.Add(PhysicalAttributes.Weight, 3);
            attributes.Add(LiftingAttributes.Limbs, 2);
            traits.Add(LiftingTraits.CanLift);
        }
    }
}