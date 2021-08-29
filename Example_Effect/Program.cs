using System;
using System.Linq;
using ExoActive;

namespace Example_Effect
{
    class Program
    {
        static void Main(string[] args)
        {
            IEntity[] team1 = {new Actor(), new Actor(), new Actor()};
            IEntity[] team2 = {new Actor(), new Actor(), new Actor()};
            
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

            Capability.PerformAction<Attack>(team1, team2);
            Capability.PerformAction<Attack>(team1, team2);
            Capability.PerformAction<Attack>(team1, team2);
            Capability.PerformAction<Heal>(team2, team2);
            
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