using System.Collections.Generic;
using ExoActive;
using NUnit.Framework;

namespace Tests
{
    public class TestCapabilityDrink : Capability
    {
        public TestCapabilityDrink() : base(CapabilityTriggerProcess<Cup>.Get(Cup.Trigger.Drink, DataSelect.Actors))
        { }
    }
    
    public class TestCapabilityFill : Capability
    {
        public TestCapabilityFill() : base(CapabilityTriggerProcess<Cup>.Get(Cup.Trigger.Fill, DataSelect.Actors))
        { }
    }

    public class CapabilityTest
    {
        [Test]
        public void CanCreate()
        {
            var cap = Capability.Get<TestCapabilityFill>();
            Assert.IsInstanceOf<TestCapabilityFill>(cap);
        }

        [Test]
        public void CanPerformAction()
        {
            var entity = new TestEntity();
            var actors = new List<IEntity> {entity};
            var data = new CapabilityProcessData(actors, null);

            Assert.False(entity.HasState<Cup>());

            Assert.False(Capability.PerformAction<TestCapabilityDrink>(data));
            Assert.True(Capability.PerformAction<TestCapabilityFill>(data));
            
            Assert.True(Capability.PerformAction<TestCapabilityDrink>(data));
            Assert.True(Capability.PerformAction<TestCapabilityFill>(data));

            Assert.AreEqual(Cup.State.HalfFull, entity.GetState<Cup>().CurrentState);

            Assert.True(Capability.PerformAction<TestCapabilityFill>(data));

            Assert.AreEqual(Cup.State.Full, entity.GetState<Cup>().CurrentState);

            Assert.False(Capability.PerformAction<TestCapabilityFill>(data));
            Assert.True(Capability.PerformAction<TestCapabilityDrink>(data));

            Assert.AreEqual(Cup.State.HalfFull, entity.GetState<Cup>().CurrentState);

            Assert.True(Capability.PerformAction<TestCapabilityFill>(data));
            Assert.True(Capability.PerformAction<TestCapabilityDrink>(data));
            Assert.True(Capability.PerformAction<TestCapabilityDrink>(data));

            Assert.AreEqual(Cup.State.Empty, entity.GetState<Cup>().CurrentState);
        }
    }
}