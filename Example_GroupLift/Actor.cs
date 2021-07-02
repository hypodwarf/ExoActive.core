using System;
using static ExoActive.ExoActive<System.Enum, int>;

namespace Example_GroupLift
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