using System;
using ExoActive;
using NUnit.Framework;
using Object = ExoActive.Object;

namespace Tests
{
    
    public class Cup : State
    {
        public enum State
        {
            Empty,
            HalfFull,
            Full
        }

        public enum Trigger
        {
            Fill,
            Drink
        }
        
        public override Array States { get => Enum.GetValues(typeof(State)); }
        public override Array Triggers { get => Enum.GetValues(typeof(Trigger)); }
        
        public Cup(State? initialState = null) : base("StateContainer", initialState ?? State.Empty)
        {
            machine.Configure(State.Empty)
                .OnEntry(() => Console.WriteLine("Empty"))
                .Permit(Trigger.Fill, State.HalfFull);
            
            machine.Configure(State.HalfFull)
                .OnEntryFrom(Trigger.Fill, () => Console.WriteLine("Inc"))
                .OnEntryFrom(Trigger.Drink, () => Console.WriteLine("Dec"))
                .Permit(Trigger.Fill, State.Full)
                .Permit(Trigger.Drink, State.Empty);
            
            machine.Configure(State.Full)
                .OnEntry(() => Console.WriteLine("Full"))
                .Permit(Trigger.Drink, State.HalfFull);
        }
    }
    
    public class ObjectTest
    {
        [Test]
        public void test2()
        {
            var c = new Cup();
            c.Fire(Cup.Trigger.Fill);
            c.Fire(Cup.Trigger.Fill);
            c.Fire(Cup.Trigger.Drink);
            c.Fire(Cup.Trigger.Drink);
            
            foreach (var t in c.States)
            {
                Console.WriteLine(t); 
            }
        }

        // [Flags]
        public enum Flag1 : byte
        {
            None = 0,
            One = 1 << 0,
            Two = 1 << 1,
            Three = 1 << 2,
            Four = 1 << 3,
            Five = 1 << 4,
            Six = 1 << 5,
            Seven = 1 << 6,
            Eight = 1 << 7,
        }

        // [Flags]
        public enum Flag2 : UInt64 {
            None = 0,
            One = 1UL << 0,
            Two = 1UL << 1,
            Three = 1UL << 2,
            Four = 1UL << 3,
            Five = 1UL << 4,
            Six = 1UL << 5,
            Seven = 1UL << 6,
            Eight = 1UL << 7,
            Nine = 1UL << 8,
            Ten = 1UL << 9,
            TenOne = 1UL << 10,
            TenTwo = 1UL << 11,
            TenThree = 1UL << 12,
            TenFour = 1UL << 13,
            TenFive = 1UL << 14,
            TenSix = 1UL << 15,
            TenSeven = 1UL << 16,
            TenEight = 1UL << 17,
            TenNine = 1UL << 18,
            TenTen = 1UL << 19,
            TenTenOne = 1UL << 20,
            TenTenTwo = 1UL << 21,
            TenTenThree = 1UL << 22,
            TenTenFour = 1UL << 23,
            TenTenFive = 1UL << 24,
            TenTenSix = 1UL << 25,
            TenTenSeven = 1UL << 26,
            TenTenEight = 1UL << 27,
            TenTenNine = 1UL << 28,
            TenTenTen = 1UL << 29,
            TenTenTenOne = 1UL << 30,
            TenTenTenTwo = 1UL << 31,
            TenTenTenThree = 1UL << 32,
            TenTenTenFour = 1UL << 33,
            TenTenTenFive = 1UL << 34,
            TenTenTenSix = 1UL << 35,
            TenTenTenSeven = 1UL << 36,
            TenTenTenEight = 1UL << 37,
            TenTenTenNine = 1UL << 38,
            TenTenTenTen = 1UL << 39,
            TenTenTenTenOne = 1UL << 40,
            TenTenTenTenTwo = 1UL << 41,
            TenTenTenTenThree = 1UL << 42,
            TenTenTenTenFour = 1UL << 43,
            TenTenTenTenFive = 1UL << 44,
            TenTenTenTenSix = 1UL << 45,
            TenTenTenTenSeven = 1UL << 46,
            TenTenTenTenEight = 1UL << 47,
            TenTenTenTenNine = 1UL << 48,
            TenTenTenTenTen = 1UL << 49,
            TenTenTenTenTenOne = 1UL << 50,
            TenTenTenTenTenTwo = 1UL << 51,
            TenTenTenTenTenThree = 1UL << 52,
            TenTenTenTenTenFour = 1UL << 53,
            TenTenTenTenTenFive = 1UL << 54,
            TenTenTenTenTenSix = 1UL << 55,
            TenTenTenTenTenSeven = 1UL << 56,
            TenTenTenTenTenEight = 1UL << 57,
            TenTenTenTenTenNine = 1UL << 58,
            TenTenTenTenTenTen = 1UL << 59,
            TenTenTenTenTenTenOne = 1UL << 60,
            TenTenTenTenTenTenTwo = 1UL << 61,
            TenTenTenTenTenTenThree = 1UL << 62,
            TenTenTenTenTenTenFour = 1UL << 63,
        }
        
        [Test]
        public void testing()
        {
            // var o = new Object();

            // Console.WriteLine(o.GetState());
            // foreach (var t in o.GetTriggers())
            // {
            //     Console.WriteLine(t); 
            // }
            
            // foreach(var e in Enum.GetValues<Flag>()) {
            //     Console.WriteLine("{0} = {1}", e, (ushort)e);
            // }

            Enum a = Flag1.None;
            Enum b = Flag2.TenTenTenTenTenTenTwo;
            Console.WriteLine(Convert.ToInt64(b));

        }
    }
}