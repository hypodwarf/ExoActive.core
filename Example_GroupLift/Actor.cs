using System;
using ExoActive;

namespace Example_GroupLift
{
    public class Actor : Manager.ManagedEntity
    {
        private static int seed = new Random().Next();
        private static Random rand = new Random(seed);

        private static void SetSeed(int val){
            seed = val;
            rand = new Random(seed);
        }
        
        public static int Seed
        {
            get => seed;
            set => SetSeed(value);
        }
        
        public Actor()
        {
            attributes.Add(PhysicalAttributes.Strength, rand.Next(2,10));
            attributes.Add(PhysicalAttributes.Weight, 3);
            traits.Add(LiftingTraits.CanLift);
        }
    }
}