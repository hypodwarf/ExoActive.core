using System;
using System.Linq;
using static ExoActive.ExoActive<System.Enum, long>;

namespace Example_Effect
{
    class Program
    {
        static void Main(string[] args)
        {
            Entity[] team1 = {new Actor(), new Actor(), new Actor()};
            Entity[] team2 = {new Actor(), new Actor(), new Actor()};
            
            Console.WriteLine("Actors can damage or heal other actors based on their weapon power");
            Console.WriteLine("--- Team 1 ---");
            foreach (var actor in team1)
            {
                Console.WriteLine($"Health: {actor.Attributes.GetAttributeValue(HealthAttributes.Health)}");
                AttributeHelper.PrintAttributeTree(actor.Attributes[HealthAttributes.Health]);
            }
            Console.WriteLine("--- Team 2 ---");
            foreach (var actor in team2)
            {
                Console.WriteLine($"Health: {actor.Attributes.GetAttributeValue(HealthAttributes.Health)}");
                AttributeHelper.PrintAttributeTree(actor.Attributes[HealthAttributes.Health]);
            }

            Capability.Get<Attack>().PerformAction(team1.ToList(), team2.ToList());
            Capability.Get<Attack>().PerformAction(team1.ToList(), team2.ToList());
            Capability.Get<Attack>().PerformAction(team1.ToList(), team2.ToList());
            Capability.Get<Heal>().PerformAction(team2.ToList(), team2.ToList());
            
            Console.WriteLine("--- Team 1 ---");
            foreach (var actor in team1)
            {
                Console.WriteLine($"Health: {actor.Attributes.GetAttributeValue(HealthAttributes.Health)}");
                AttributeHelper.PrintAttributeTree(actor.Attributes[HealthAttributes.Health]);
            }
            Console.WriteLine("--- Team 2 ---");
            foreach (var actor in team2)
            {
                Console.WriteLine($"Health: {actor.Attributes.GetAttributeValue(HealthAttributes.Health)}");
                AttributeHelper.PrintAttributeTree(actor.Attributes[HealthAttributes.Health]);
            }
        }
    }
}