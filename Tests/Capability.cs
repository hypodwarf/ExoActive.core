using System.Collections.Generic;
using System.Linq;
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

        public override void PerformAction(Object obj)
        {
            obj.State(TestObj.States.Cup).Fire(Cup.Trigger.Fill);
        }
    }
    
    public class TestCapabilityActionDrink : CapabilityAction
    {
        public TestCapabilityActionDrink()
        {
            Requirements.Add(RequirementTest.CanDrink);
        }

        public override void PerformAction(Object obj)
        {
            obj.State(TestObj.States.Cup).Fire(Cup.Trigger.Drink);
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
            var obj = new TestObj();
            var actors = new List<Object>{obj};
            var fill = new TestCapabilityFill();
            var drink = new TestCapabilityDrink();
            
            Assert.AreEqual(Cup.State.Empty, obj.State(TestObj.States.Cup).CurrentState);
            
            Assert.False(drink.PerformAction(actors));
            Assert.True(fill.PerformAction(actors));
            
            Assert.AreEqual(Cup.State.HalfFull, obj.State(TestObj.States.Cup).CurrentState);
            
            Assert.True(fill.PerformAction(actors));
            
            Assert.AreEqual(Cup.State.Full, obj.State(TestObj.States.Cup).CurrentState);
            
            Assert.False(fill.PerformAction(actors));
            Assert.True(drink.PerformAction(actors));
            
            Assert.AreEqual(Cup.State.HalfFull, obj.State(TestObj.States.Cup).CurrentState);
            
            Assert.True(drink.PerformAction(actors));
            
            Assert.AreEqual(Cup.State.Empty, obj.State(TestObj.States.Cup).CurrentState);
        }
        
        
    }
}