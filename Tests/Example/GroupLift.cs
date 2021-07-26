using System;
using Example_GroupLift;
using ExoActive;
using NUnit.Framework;

namespace Tests.Example
{
    [TestFixture]
    public class GroupLift
    {
        [Test]
        public void TestCapabilities()
        {
            Actor.Seed = 2004368288;
            
            IEntity[] actors = { new Actor(), new Actor(), new Actor(), new Actor(), new Actor(), new Actor(), new Actor(), new Actor(), new Actor()};
            IEntity item1 = new Item();

            var weightAttr = new Attributes();
            weightAttr.Add(PhysicalAttributes.Weight, 40);
            item1.Attributes.Apply(weightAttr);
            
            Assert.AreEqual(42, item1.Attributes.GetAttributeValue(PhysicalAttributes.Weight));
            Assert.AreEqual("Released" ,item1.GetState<Lift.LiftedState>().CurrentState.ToString());
            
            Capability.PerformAction<Lift.PickUp>(actors, item1);
            Assert.AreEqual(0, item1.Attributes.GetAttributeValue(PhysicalAttributes.Weight));
            Assert.AreEqual("Aloft" ,item1.GetState<Lift.LiftedState>().CurrentState.ToString());

            Capability.PerformAction<Lift.PutDown>(actors[3..5], item1);
            Assert.AreEqual(8, item1.Attributes.GetAttributeValue(PhysicalAttributes.Weight));
            Assert.AreEqual("Grappled" ,item1.GetState<Lift.LiftedState>().CurrentState.ToString());
        }
    }
}