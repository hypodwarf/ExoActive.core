using System.Collections.Generic;
using ExoActive;
using NUnit.Framework;

namespace Tests
{
    public class TestCapabilityActionFill : CapabilityAction
    {
        public TestCapabilityActionFill()
        {
           Requirements.Add(RequirementTest.CanFill);
        }

        protected override string StateId { get => typeof(Cup).FullName; }
        protected override State CreateState()
        {
            return new Cup();
        }

        public override void PerformAction(Object obj)
        {
            obj.State(StateId).Fire(Cup.Trigger.Fill);
        }
    }
    
    public class TestCapabilityActionDrink : CapabilityAction
    {
        public TestCapabilityActionDrink()
        {
            Requirements.Add(RequirementTest.CanDrink);
        }
        
        protected override string StateId { get => typeof(Cup).FullName; }
        protected override State CreateState()
        {
            return new Cup();
        }

        public override void PerformAction(Object obj)
        {
            obj.State(StateId).Fire(Cup.Trigger.Drink);
        }
    }

    public class TestCapabilityDrink : Capabilty
    {
        public TestCapabilityDrink()
        {
            actorActions.Add(new TestCapabilityActionDrink());
        }
    }
    
    public class TestCapabilityFill : Capabilty
    {
        public TestCapabilityFill()
        {
            actorActions.Add(new TestCapabilityActionFill());
        }
    }
    
    public class CapabilityTest
    {
        [Test]
        public void CanCreate()
        {
            var _ = new TestCapabilityFill();
            Assert.Pass();
        }

        [Test]
        public void CanPerformAction()
        {
            var cupId = typeof(Cup).FullName;
            var obj = new TestObj();
            var actors = new List<Object>{obj};
            var fill = new TestCapabilityFill();
            var drink = new TestCapabilityDrink();
            
            Assert.False(obj.HasState(cupId));
            
            Assert.False(drink.PerformAction(actors));
            Assert.True(fill.PerformAction(actors));
            
            Assert.AreEqual(Cup.State.HalfFull, obj.State(cupId).CurrentState);
            
            Assert.True(fill.PerformAction(actors));
            
            Assert.AreEqual(Cup.State.Full, obj.State(cupId).CurrentState);
            
            Assert.False(fill.PerformAction(actors));
            Assert.True(drink.PerformAction(actors));
            
            Assert.AreEqual(Cup.State.HalfFull, obj.State(cupId).CurrentState);
            
            Assert.True(drink.PerformAction(actors));
            
            Assert.AreEqual(Cup.State.Empty, obj.State(cupId).CurrentState);
        }
        
        
    }
}