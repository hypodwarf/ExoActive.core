using System;
using System.Linq;
using Example_Equip;
using static Example_Equip.Equip;
using ExoActive;
using NUnit.Framework;

namespace Tests.Example
{
    [TestFixture]
    public class Equip
    {
        [Test]
        public void TestCapabilities() 
        {
            Actor actor = new Actor();
            Ring ring1 = new Ring();
            Ring ring2 = new Ring();
            Ring ring3 = new Ring();
            Belt belt1 = new Belt();
            Belt belt2 = new Belt();
            
            Assert.AreEqual(10, actor.Attributes.GetAttributeValue(PhysicalAttributes.Strength));
            Assert.AreEqual(3, actor.Attributes.GetAttributeValue(PhysicalAttributes.Weight));

            Assert.IsTrue(Capability.PerformAction<EquipItem>(actor, ring1));
            
            Assert.AreEqual(12, actor.Attributes.GetAttributeValue(PhysicalAttributes.Strength));
            Assert.AreEqual(4, actor.Attributes.GetAttributeValue(PhysicalAttributes.Weight));
            
            Assert.IsTrue(Capability.PerformAction<EquipItem>(actor, belt1));
            
            Assert.AreEqual(17, actor.Attributes.GetAttributeValue(PhysicalAttributes.Strength));
            Assert.AreEqual(7, actor.Attributes.GetAttributeValue(PhysicalAttributes.Weight));
            
            Assert.IsFalse(Capability.PerformAction<EquipItem>(actor, belt2));
            Assert.IsFalse(Capability.PerformAction<EquipItem>(actor, ring1));
            
            Assert.AreEqual(17, actor.Attributes.GetAttributeValue(PhysicalAttributes.Strength));
            Assert.AreEqual(7, actor.Attributes.GetAttributeValue(PhysicalAttributes.Weight));
            
            Assert.IsTrue(Capability.PerformAction<EquipItem>(actor, ring2));
            
            Assert.AreEqual(19, actor.Attributes.GetAttributeValue(PhysicalAttributes.Strength));
            Assert.AreEqual(8, actor.Attributes.GetAttributeValue(PhysicalAttributes.Weight));
            
            Assert.IsFalse(Capability.PerformAction<EquipItem>(actor, ring3));
            
            Assert.AreEqual(19, actor.Attributes.GetAttributeValue(PhysicalAttributes.Strength));
            Assert.AreEqual(8, actor.Attributes.GetAttributeValue(PhysicalAttributes.Weight));
            
            Assert.IsTrue(Capability.PerformAction<UnequipItem>(actor, ring1));
            
            Assert.AreEqual(17, actor.Attributes.GetAttributeValue(PhysicalAttributes.Strength));
            Assert.AreEqual(7, actor.Attributes.GetAttributeValue(PhysicalAttributes.Weight));
            
            Assert.IsTrue(Capability.PerformAction<EquipItem>(actor, ring3));
            
            Assert.AreEqual(19, actor.Attributes.GetAttributeValue(PhysicalAttributes.Strength));
            Assert.AreEqual(8, actor.Attributes.GetAttributeValue(PhysicalAttributes.Weight));
        }
    }
}