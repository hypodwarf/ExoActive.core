using System;
using ExoActive;

namespace Example_Equip
{
    class Program
    {
        static void Main(string[] args)
        {
            Actor actor = new Actor();
            Ring ring1 = new Ring();
            Ring ring2 = new Ring();
            Ring ring3 = new Ring();
            Belt belt1 = new Belt();
            Belt belt2 = new Belt();

            Console.WriteLine(Capability.PerformAction<Equip.EquipItem>(actor, ring1));
            Console.WriteLine(Capability.PerformAction<Equip.EquipItem>(actor, ring2));
            Console.WriteLine(Capability.PerformAction<Equip.EquipItem>(actor, ring3));
            Console.WriteLine(Capability.PerformAction<Equip.EquipItem>(actor, belt1));
            Console.WriteLine(Capability.PerformAction<Equip.EquipItem>(actor, belt2));
            
            Console.WriteLine(Capability.PerformAction<Equip.UnequipItem>(actor, ring1));
            Console.WriteLine(Capability.PerformAction<Equip.UnequipItem>(actor, ring2));
            Console.WriteLine(Capability.PerformAction<Equip.UnequipItem>(actor, ring3));
            Console.WriteLine(Capability.PerformAction<Equip.UnequipItem>(actor, belt1));
            Console.WriteLine(Capability.PerformAction<Equip.UnequipItem>(actor, belt2));
        }
    }
}