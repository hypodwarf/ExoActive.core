using System.Collections.Generic;
using static ExoActive.ExoActive<System.Enum, int>;
using NUnit.Framework;

namespace Tests
{
    // Creating the Capability using the CapabilityTriggerProcess
    public class TestCapabilityDrink : Capability
    {
        public TestCapabilityDrink() : base(
            new ICapabilityProcess[] { CapabilityTriggerProcess<Cup>.Get(Cup.Trigger.Drink) })
        { }
    }

    // Creating the Capability using the CapabilityStateProcess
    public class TestCapabilityFill : Capability
    {
        private static readonly CapabilityStateProcess<Cup> TriggerProcess = CapabilityStateProcess<Cup>.Create(
            new[]
            {
                CapabilityStateProcess<Cup>.FireAction(Cup.Trigger.Fill)
            },
            new[]
            {
                RequirementTest.CanFill
            }
        );

        public TestCapabilityFill() : base(new ICapabilityProcess[] {TriggerProcess})
        {
        }
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
            var actors = new List<Entity> {entity};
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
            Assert.AreEqual(1, entity.GetState<Cup>().Entities.Count);

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