using System;
using ExoActive;
using NUnit.Framework;

namespace Tests
{
    public class AttributeGroupTests
    {
        enum types
        {
            T0, T1, T2, T3
        }
        
        [Test]
        public void CanCreate()
        {
            var g1 = new AttributeGroup<string, int>();
            var g2 = new AttributeGroup<types, string>();
            
            Assert.Pass();
        }

        [Test]
        public void CanManageAttributes()
        {
            var g = new AttributeGroup<types, int>();
            
            Assert.False(g.HasAttribute(types.T0));
            
            g.AddAttribute(types.T0);
            g.AddAttribute(types.T1, 1);
            g.AddAttribute(types.T2, 2, "T-Two");
            
            Assert.True(g.HasAttribute(types.T0));
            Assert.AreEqual (0, g.GetAttributeValue(types.T0));
            Assert.AreEqual (1, g.GetAttributeValue(types.T1));
            Assert.AreEqual (2, g.GetAttributeValue(types.T2));
            
            g.RemoveAttribute(types.T0);
            Assert.False(g.HasAttribute(types.T0));
        }

        [Test]
        public void CanModifyAttributes()
        {
            var g1 = new AttributeGroup<types, int>();
            var g2 = new AttributeGroup<types, int>();
            
            g1.AddAttribute(types.T0);
            g1.AddAttribute(types.T1, 1);
            g1.AddAttribute(types.T2, 2, "T-Two");
            
            g2.AddAttribute(types.T0, 10);
            g2.AddAttribute(types.T1, 11);
            g2.AddAttribute(types.T2, 12, "Tootsie");
            
            Assert.IsEmpty(g1.ApplyModification(g2));
            Assert.AreEqual (10, g1.GetAttributeValue(types.T0));
            Assert.AreEqual (12, g1.GetAttributeValue(types.T1));
            Assert.AreEqual (14, g1.GetAttributeValue(types.T2));
            
            Assert.AreEqual (10, g2.GetAttributeValue(types.T0));
            Assert.AreEqual (11, g2.GetAttributeValue(types.T1));
            Assert.AreEqual (12, g2.GetAttributeValue(types.T2));
            
            Assert.IsEmpty(g1.RemoveModification(g2));
            Assert.AreEqual (0, g1.GetAttributeValue(types.T0));
            Assert.AreEqual (1, g1.GetAttributeValue(types.T1));
            Assert.AreEqual (2, g1.GetAttributeValue(types.T2));
            
            Assert.AreEqual (10, g2.GetAttributeValue(types.T0));
            Assert.AreEqual (11, g2.GetAttributeValue(types.T1));
            Assert.AreEqual (12, g2.GetAttributeValue(types.T2));
        }

        [Test]
        public void ReportMissingAttributes()
        {
            var g1 = new AttributeGroup<types, int>();
            var g2 = new AttributeGroup<types, int>();
            
            g1.AddAttribute(types.T0);
            g1.AddAttribute(types.T1, 1);
            
            g2.AddAttribute(types.T0, 10);
            g2.AddAttribute(types.T1, 11);
            g2.AddAttribute(types.T2, 12, "Tootsie");

            var missing = g1.ApplyModification(g2);
            Assert.AreEqual(1,missing.Count);
            Assert.AreEqual (10, g1.GetAttributeValue(types.T0));
            Assert.AreEqual (12, g1.GetAttributeValue(types.T1));
            
            missing = g1.RemoveModification(g2);
            Assert.AreEqual(1,missing.Count);
            Assert.AreEqual (0, g1.GetAttributeValue(types.T0));
            Assert.AreEqual (1, g1.GetAttributeValue(types.T1));
        }

        [Test]
        public void CanAddCircularModifications()
        {
            var g1 = new AttributeGroup<types, int>();
            var g2 = new AttributeGroup<types, int>();
            
            g1.AddAttribute(types.T0);
            g1.AddAttribute(types.T1, 1);
            g1.AddAttribute(types.T2, 2, "T-Two");
            
            g2.AddAttribute(types.T0, 10);
            g2.AddAttribute(types.T1, 11);
            g2.AddAttribute(types.T2, 12, "Tootsie");

            g1.ApplyModification(g2);
            g2.ApplyModification(g1);
            
            Assert.AreEqual (10, g1.GetAttributeValue(types.T0));
            Assert.AreEqual (12, g1.GetAttributeValue(types.T1));
            Assert.AreEqual (14, g1.GetAttributeValue(types.T2));
            
            Assert.AreEqual (20, g2.GetAttributeValue(types.T0));
            Assert.AreEqual (23, g2.GetAttributeValue(types.T1));
            Assert.AreEqual (26, g2.GetAttributeValue(types.T2));

            g1.RemoveModification(g2);
            Assert.AreEqual (0, g1.GetAttributeValue(types.T0));
            Assert.AreEqual (1, g1.GetAttributeValue(types.T1));
            Assert.AreEqual (2, g1.GetAttributeValue(types.T2));
            
            g1 = new AttributeGroup<types, int>();
            g2 = new AttributeGroup<types, int>();
            
            g1.AddAttribute(types.T0);
            g1.AddAttribute(types.T1, 1);
            g1.AddAttribute(types.T2, 2, "T-Two");
            
            g2.AddAttribute(types.T0, 10);
            g2.AddAttribute(types.T1, 11);
            g2.AddAttribute(types.T2, 12, "Tootsie");

            g2.ApplyModification(g1);
            g1.ApplyModification(g2);
            
            Assert.AreEqual (10, g1.GetAttributeValue(types.T0));
            Assert.AreEqual (13, g1.GetAttributeValue(types.T1));
            Assert.AreEqual (16, g1.GetAttributeValue(types.T2));
            
            Assert.AreEqual (10, g2.GetAttributeValue(types.T0));
            Assert.AreEqual (12, g2.GetAttributeValue(types.T1));
            Assert.AreEqual (14, g2.GetAttributeValue(types.T2));

            g1.RemoveModification(g2);
            Assert.AreEqual (0, g1.GetAttributeValue(types.T0));
            Assert.AreEqual (1, g1.GetAttributeValue(types.T1));
            Assert.AreEqual (2, g1.GetAttributeValue(types.T2));

        }
    }
}