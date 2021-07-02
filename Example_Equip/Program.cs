using System;
using static ExoActive.ExoActive<System.Enum, int>;

namespace Example_Equip
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("An actor is capable of equipping 2 ring and 1 belt.");
            
            Actor actor = new Actor();
            Ring ring1 = new Ring();
            Ring ring2 = new Ring();
            Ring ring3 = new Ring();
            Belt belt1 = new Belt();
            Belt belt2 = new Belt();

            Console.WriteLine($"Equipped 1st ring: {Capability.PerformAction<Equip.EquipItem>(actor, ring1)}");
            Console.WriteLine($"Equipped 2nd ring: {Capability.PerformAction<Equip.EquipItem>(actor, ring2)}");
            Console.WriteLine($"Equipped 3rd ring: {Capability.PerformAction<Equip.EquipItem>(actor, ring3)}");
            Console.WriteLine($"Equipped 1st belt: {Capability.PerformAction<Equip.EquipItem>(actor, belt1)}");
            Console.WriteLine($"Equipped 2nd belt: {Capability.PerformAction<Equip.EquipItem>(actor, belt2)}");
            
            Console.WriteLine($"Unequipped 1st ring: {Capability.PerformAction<Equip.UnequipItem>(actor, ring1)}");
            Console.WriteLine($"Equipped 3rd ring: {Capability.PerformAction<Equip.EquipItem>(actor, ring3)}");
            
            Console.WriteLine($"Unequipped 1st ring: {Capability.PerformAction<Equip.UnequipItem>(actor, ring1)}");
            Console.WriteLine($"Unequipped 2nd ring: {Capability.PerformAction<Equip.UnequipItem>(actor, ring2)}");
            Console.WriteLine($"Unequipped 3rd ring: {Capability.PerformAction<Equip.UnequipItem>(actor, ring3)}");
            Console.WriteLine($"Unequipped 1st belt: {Capability.PerformAction<Equip.UnequipItem>(actor, belt1)}");
            Console.WriteLine($"Unequipped 2nd belt: {Capability.PerformAction<Equip.UnequipItem>(actor, belt2)}");
        }
    }
}