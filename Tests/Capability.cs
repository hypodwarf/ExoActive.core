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
            var fill = Capability.Get<TestCapabilityFill>();
            var drink = Capability.Get<TestCapabilityDrink>();

            Assert.False(entity.HasState<Cup>());

            Assert.False(drink.PerformAction(actors));
            Assert.True(fill.PerformAction(actors));
            
            Assert.True(drink.PerformAction(actors));
            Assert.True(fill.PerformAction(actors));

            Assert.AreEqual(Cup.State.HalfFull, entity.GetState<Cup>().CurrentState);

            Assert.True(fill.PerformAction(actors));

            Assert.AreEqual(Cup.State.Full, entity.GetState<Cup>().CurrentState);

            Assert.False(fill.PerformAction(actors));
            Assert.True(drink.PerformAction(actors));

            Assert.AreEqual(Cup.State.HalfFull, entity.GetState<Cup>().CurrentState);

            Assert.True(fill.PerformAction(actors));
            Assert.True(drink.PerformAction(actors));
            Assert.True(drink.PerformAction(actors));

            Assert.AreEqual(Cup.State.Empty, entity.GetState<Cup>().CurrentState);
        }
    }
}