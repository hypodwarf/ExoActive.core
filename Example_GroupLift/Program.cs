using System;
using static ExoActive.Type<System.Enum, int>;

namespace Example_GroupLift
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Entity[] actors = { new Actor(), new Actor(), new Actor(), new Actor(), new Actor(), new Actor(), new Actor(), new Actor(), new Actor()};
            Entity item1 = new Item();
            Entity item2 = new Item();
            
            // var strengthAttr = new Attributes();
            // strengthAttr.Add(PhysicalAttributes.Strength, -6);
            //
            // actors[2].Attributes.Apply(strengthAttr);
            
            var weightAttr = new Attributes();
            weightAttr.Add(PhysicalAttributes.Weight, 40);

            item1.Attributes.Apply(weightAttr);

            Array.ForEach(actors, actor => Console.WriteLine($"Actor {actor.guid} -> Strength: {actor.Attributes.GetAttributeValue(PhysicalAttributes.Strength)} LiftingState: {actor.GetState<Lift.LiftingState>().CurrentState}"));
            Console.WriteLine($"Item {item1.guid} -> Weight: {item1.Attributes.GetAttributeValue(PhysicalAttributes.Weight)} LiftedState: {item1.GetState<Lift.LiftedState>().CurrentState}");

            Capability.PerformAction<Lift.PickUp>(actors, item1);
            
            Array.ForEach(actors, actor => Console.WriteLine($"Actor {actor.guid} -> Strength: {actor.Attributes.GetAttributeValue(PhysicalAttributes.Strength)} LiftingState: {actor.GetState<Lift.LiftingState>().CurrentState}"));
            Console.WriteLine($"Item {item1.guid} -> Weight: {item1.Attributes.GetAttributeValue(PhysicalAttributes.Weight)} LiftedState: {item1.GetState<Lift.LiftedState>().CurrentState}");

            Capability.PerformAction<Lift.PutDown>(actors[3..5], item1);
            
            Array.ForEach(actors, actor => Console.WriteLine($"Actor {actor.guid} -> Strength: {actor.Attributes.GetAttributeValue(PhysicalAttributes.Strength)} LiftingState: {actor.GetState<Lift.LiftingState>().CurrentState}"));
            Console.WriteLine($"Item {item1.guid} -> Weight: {item1.Attributes.GetAttributeValue(PhysicalAttributes.Weight)} LiftedState: {item1.GetState<Lift.LiftedState>().CurrentState}");

        }
    }
}