using System;
using ExoActive;

namespace Example
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var actor = new Actor();
            var item = new Item[]
            {
                new Item(), new Item(), new Item()
            };

            Capability.PerformAction<PickUp>(actor, item[0]);
            Console.WriteLine(actor.GetState<HoldState>().Entities);
            Console.WriteLine(item[0].GetState<HeldState>().Entities);
            Console.WriteLine(item[0].GetState<HeldState>().CurrentState);
            Capability.PerformAction<PickUp>(actor, item[1]);
            Console.WriteLine(actor.GetState<HoldState>().Entities);
            Console.WriteLine(item[1].GetState<HeldState>().Entities);
            Console.WriteLine(item[1].GetState<HeldState>().CurrentState);
            Capability.PerformAction<PutDown>(actor, item[0]);
            Console.WriteLine(actor.GetState<HoldState>().Entities);
            Console.WriteLine(item[0].GetState<HeldState>().Entities);
            Console.WriteLine(item[0].GetState<HeldState>().CurrentState);
        }
    }
}