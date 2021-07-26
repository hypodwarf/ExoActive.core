using System;
using ExoActive;

namespace Example_GroupCombined
{
    /***
     * A battle to lift the rock!
     * Two teams of actors. The first team to lift the rock wins! Actros can use their strength to help lift the rock,
     * but they can also attack their oppenents, heal their team mates or pickup magical items are also available that
     * increase the wearers strength.
     */
    
    class Program
    {
        static void PrintActors(params IEntity[] entities)
        {
            Array.ForEach(entities, actor => Console.WriteLine($"Actor {actor.Guid} -> Strength: {actor.Attributes.GetAttributeValue(PhysicalAttributes.Strength)} Health: {actor.Attributes.GetAttributeValue(PhysicalAttributes.Health)} LiftingState: {actor.GetState<Lift.LiftingState>().CurrentState}"));
        }

        static void PrintItems(params IEntity[] entities)
        {
            Array.ForEach(entities, entity =>
                Console.WriteLine(
                    $"Item {entity.Guid} -> Weight: {entity.Attributes.GetAttributeValue(PhysicalAttributes.Weight)} LiftedState: {entity.GetState<Lift.LiftedState>().CurrentState}"));
        }

        static void PrintEquipment(params IEntity[] entities)
        {
            Array.ForEach(entities, entity =>
                Console.WriteLine(
                    $"{entity.Traits.Value<EquipmentTraits>()} {entity.Guid} -> Strength: {entity.Attributes.GetAttributeValue(PhysicalAttributes.Strength)} EquippeddState: {entity.GetState<Equip.ItemEquippedState>().CurrentState}"));
        }
        
        static void Main(string[] args)
        {
            IEntity[] team1 = { new Actor(), new Actor(), new Actor(), new Actor(), new Actor(), new Actor(), new Actor(), new Actor(), new Actor()};
            IEntity[] team2 = { new Actor(), new Actor(), new Actor(), new Actor(), new Actor(), new Actor(), new Actor(), new Actor(), new Actor()};
            IEntity item1 = new Item();
            IEntity item2 = new Item();
            IEntity ring1 = new Ring();
            IEntity ring2 = new Ring();
            IEntity ring3 = new Ring();
            IEntity belt1 = new Belt();
            IEntity belt2 = new Belt();
            
            PrintActors(team1);
            PrintItems(item1);
            PrintEquipment(ring1, ring2, ring3, belt1, belt2);

            Capability.PerformAction<Lift.PickUp>(team1, item1);
            
            PrintActors(team1);
            PrintItems(item1);

            Capability.PerformAction<Equip.EquipItem>(team1[1], ring1);
            
            Capability.PerformAction<Lift.PutDown>(team1[5..], item1);
            
            Capability.PerformAction<Equip.UnequipItem>(team1[1], ring1);
            
            PrintActors(team1);
            PrintItems(item1);

            TimeTicker.AddTicks(0);
            
            PrintActors(team1);
            PrintItems(item1);
        }
    }
}