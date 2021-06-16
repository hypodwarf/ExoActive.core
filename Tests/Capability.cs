using System.Collections.Generic;
using ExoActive;
using NUnit.Framework;

namespace Tests
{
    public class TestCapabilityActionFill : CapabilityAction<Cup>
    {
        public TestCapabilityActionFill()
        {
            Requirements.Add(RequirementTest.CanFill);
            ActionEvent += entity => GetState(entity).Fire(Cup.Trigger.Fill);
        }
    }

    public class TestCapabilityActionDrink : CapabilityAction<Cup>
    {
        public TestCapabilityActionDrink()
        {
            Requirements.Add(RequirementTest.CanDrink);
        }

        public override void PerformAction(Entity entity)
        {
            GetState(entity).Fire(Cup.Trigger.Drink);
        }
    }

    public class TestCapabilityDrink : Capability
    {
        public TestCapabilityDrink() : base(new ICapabilityAction[]
        {
            new TestCapabilityActionDrink()
        })
        {
        }
    }

    public class TestCapabilityFill : Capability
    {
        private static readonly CapabilityAction<Cup> Action = CapabilityAction<Cup>.Create(
            new[]
            {
                CapabilityAction<Cup>.FireAction(Cup.Trigger.Fill)
            },
            new[]
            {
                RequirementTest.CanFill
            }
        );

        public TestCapabilityFill() : base(new ICapabilityAction[] {Action})
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

            Assert.AreEqual(Cup.State.HalfFull, entity.GetState<Cup>().CurrentState);

            Assert.True(fill.PerformAction(actors));

            Assert.AreEqual(Cup.State.Full, entity.GetState<Cup>().CurrentState);

            Assert.False(fill.PerformAction(actors));
            Assert.True(drink.PerformAction(actors));

            Assert.AreEqual(Cup.State.HalfFull, entity.GetState<Cup>().CurrentState);

            Assert.True(drink.PerformAction(actors));

            Assert.AreEqual(Cup.State.Empty, entity.GetState<Cup>().CurrentState);
        }
    }
}