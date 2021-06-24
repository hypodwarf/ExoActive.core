using System;
using System.Linq;
using ExoActive;

namespace Example
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Entity[] actor = { new Actor(), new Actor()};
            Entity[] item =
            {
                new Item(), new Item(), new Item(), new Item(), new Item(),
                new Item(), new Item(), new Item(), new Item(), new Item()
            };

            var result = new bool[6];

            result[0] = Capability.PerformAction<PickUp>(actor, item[0]);
            // result[0] = Capability.PerformAction<PickUp>(actor);
            Console.WriteLine(item[0].GetState<LiftedState>().Entities);
            // result[1] = Capability.PerformAction<PickUp>(actor, item[1], item[2], item[3]);
            // Console.WriteLine(actor[0].GetState<LiftingState>().Entities);
            // result[2] = Capability.PerformAction<PickUp>(actor, item[4], item[5]);
            // Console.WriteLine(actor[0].GetState<LiftingState>().Entities);
            // result[3] = Capability.PerformAction<PickUp>(actor);
            // Console.WriteLine(actor[0].GetState<LiftingState>().Entities);
            // result[4] = Capability.PerformAction<PutDown>(actor, item[1], item[2]);
            // Console.WriteLine(actor[0].GetState<LiftingState>().Entities);
            result[5] = Capability.PerformAction<PutDown>(actor, item[0]);
            // Console.WriteLine(actor[0].GetState<LiftingState>().Entities);
            // Console.WriteLine(actor[0].GetState<LiftingState>().CurrentState);
            
            foreach (var b in result)
            {
                Console.WriteLine(b);
            } 
        }
    }
}