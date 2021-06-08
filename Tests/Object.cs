using System;
using ExoActive;
using NUnit.Framework;
using Object = ExoActive.Object;

namespace Tests
{
    public class TestObj : Object
    {
        [Flags]
        public enum ObjChar : ulong
        {
            Happy = 1UL << 0,
            Sad = 1UL << 1
        }

        public enum ObjAttr
        {
            Strength
        }

        public enum States
        {
            Cup
        }

        public enum Capability
        {
            Fill,
            Drink
        }

        public TestObj()
        {
            // capabilities.Add(Capability.Fill, new TestCapabilityFill());
            // capabilities.Add(Capability.Drink, new TestCapabilityDrink());
            characteristics.Add(ObjChar.Happy);
            attributes.Add(ObjAttr.Strength, 10);
            states.Add(States.Cup, new Cup());
        }
    }
    
    public class ObjectTest
    {
        // [Test]
        // public void Cup()
        // {
        //     var cup = new TestObj();
        //     Assert.AreEqual(Tests.Cup.State.Empty, cup.State(TestObj.States.Cup).State);
        //     Assert.False(cup.PerformCapability(TestObj.Capability.Drink));
        //     
        //     Assert.True(cup.PerformCapability(TestObj.Capability.Fill));
        //     Assert.AreEqual(Tests.Cup.State.HalfFull, cup.State(TestObj.States.Cup).State);
        //     
        //     Assert.True(cup.PerformCapability(TestObj.Capability.Fill));
        //     Assert.AreEqual(Tests.Cup.State.Full, cup.State(TestObj.States.Cup).State);
        // }
    }
}