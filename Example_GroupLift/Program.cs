using System;
using ExoActive;

namespace Example_GroupLift
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Actor.Seed = 2004368288;
            Console.WriteLine($"Seed: {Actor.Seed}");
            
            IEntity[] actors = { new Actor(), new Actor(), new Actor(), new Actor(), new Actor(), new Actor(), new Actor(), new Actor(), new Actor()};
            IEntity item1 = new Item();
            IEntity item2 = new Item();
            
            // var strengthAttr = new Attributes();
            // strengthAttr.Add(PhysicalAttributes.Strength, -6);
            //
            // actors[2].Attributes.Apply(strengthAttr);
            
            var weightAttr = new Attributes();
            weightAttr.Add(PhysicalAttributes.Weight, 40);

            item1.Attributes.Apply(weightAttr);

            Array.ForEach(actors, actor => Console.WriteLine($"Actor {actor.Guid} -> Strength: {actor.Attributes.GetAttributeValue(PhysicalAttributes.Strength)} LiftingState: {actor.GetState<Lift.LiftingState>().CurrentState}"));
            Console.WriteLine($"Item {item1.Guid} -> Weight: {item1.Attributes.GetAttributeValue(PhysicalAttributes.Weight)} LiftedState: {item1.GetState<Lift.LiftedState>().CurrentState}");

            Capability.PerformAction<Lift.PickUp>(actors, item1);
            
            Array.ForEach(actors, actor => Console.WriteLine($"Actor {actor.Guid} -> Strength: {actor.Attributes.GetAttributeValue(PhysicalAttributes.Strength)} LiftingState: {actor.GetState<Lift.LiftingState>().CurrentState}"));
            Console.WriteLine($"Item {item1.Guid} -> Weight: {item1.Attributes.GetAttributeValue(PhysicalAttributes.Weight)} LiftedState: {item1.GetState<Lift.LiftedState>().CurrentState}");

            Capability.PerformAction<Lift.PutDown>(actors[3..5], item1);
            
            Array.ForEach(actors, actor => Console.WriteLine($"Actor {actor.Guid} -> Strength: {actor.Attributes.GetAttributeValue(PhysicalAttributes.Strength)} LiftingState: {actor.GetState<Lift.LiftingState>().CurrentState}"));
            Console.WriteLine($"Item {item1.Guid} -> Weight: {item1.Attributes.GetAttributeValue(PhysicalAttributes.Weight)} LiftedState: {item1.GetState<Lift.LiftedState>().CurrentState}");

        }
    }
}