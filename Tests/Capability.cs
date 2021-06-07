using System.Linq;
using ExoActive;
using NUnit.Framework;

namespace Tests
{
    public class TestCapabilityFill : Capability
    {
        public TestCapabilityFill() : base(false, false)
        {
           OwnerRequirements.Add(RequirementTest.CanFill);
        }

        protected override bool Action(Object owner, Object target = null)
        {
            owner.State(TestObj.States.Cup).Fire(Cup.Trigger.Fill);
            return true;
        }
    }
    
    public class TestCapabilityDrink : Capability
    {
        public TestCapabilityDrink() : base(false, false)
        {
            OwnerRequirements.Add(RequirementTest.CanDrink);
        }

        protected override bool Action(Object owner, Object target = null)
        {
            owner.State(TestObj.States.Cup).Fire(Cup.Trigger.Drink);
            return true;
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
        public void AttachedToObject()
        {
            var obj = new TestObj();
            
            Assert.AreEqual(Cup.State.Empty, obj.State(TestObj.States.Cup).CurrentState);

            Assert.Contains(TestObj.Capability.Fill, obj.Capabilities().ToList());
            Assert.Contains(TestObj.Capability.Drink, obj.Capabilities().ToList());
            
            Assert.True(obj.Capabilities(TestObj.Capability.Fill).CanPerform(obj));
            Assert.False(obj.Capabilities(TestObj.Capability.Drink).CanPerform(obj));
            
            
        }
    }
}