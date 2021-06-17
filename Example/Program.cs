using System;
using ExoActive;

namespace Example
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var actor = new Actor();
            Capability.PerformAction<PutDown>(actor);
            Capability.PerformAction<PickUp>(actor);
            Console.WriteLine(actor.GetState<HoldState>().CurrentState);
        }
    }
}