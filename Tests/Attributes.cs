using System;
using ExoActive;
using NUnit.Framework;

namespace Tests
{
    public class AttributeGroupTests
    {
        public enum types
        {
            T0,
            T1,
            T2,
            T3
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

            Assert.False(g.Has(types.T0));

            g.Add(types.T0);
            g.Add(types.T1, 1);
            g.Add(types.T2, 2, "T-Two");

            Assert.True(g.Has(types.T0));
            Assert.AreEqual(0, g.GetAttributeValue(types.T0));
            Assert.AreEqual(1, g.GetAttributeValue(types.T1));
            Assert.AreEqual(2, g.GetAttributeValue(types.T2));

            g.Remove(types.T0);
            Assert.False(g.Has(types.T0));
        }

        [Test]
        public void CanModifyAttributes()
        {
            var g1 = new AttributeGroup<types, int>();
            var g2 = new AttributeGroup<types, int>();

            g1.Add(types.T0);
            g1.Add(types.T1, 1);
            g1.Add(types.T2, 2, "T-Two");

            g2.Add(types.T0, 10);
            g2.Add(types.T1, 11);
            g2.Add(types.T2, 12, "Tootsie");

            Assert.IsEmpty(g1.Apply(g2));
            Assert.AreEqual(10, g1.GetAttributeValue(types.T0));
            Assert.AreEqual(12, g1.GetAttributeValue(types.T1));
            Assert.AreEqual(14, g1.GetAttributeValue(types.T2));

            Assert.AreEqual(10, g2.GetAttributeValue(types.T0));
            Assert.AreEqual(11, g2.GetAttributeValue(types.T1));
            Assert.AreEqual(12, g2.GetAttributeValue(types.T2));

            Assert.IsEmpty(g1.Revert(g2));
            Assert.AreEqual(0, g1.GetAttributeValue(types.T0));
            Assert.AreEqual(1, g1.GetAttributeValue(types.T1));
            Assert.AreEqual(2, g1.GetAttributeValue(types.T2));

            Assert.AreEqual(10, g2.GetAttributeValue(types.T0));
            Assert.AreEqual(11, g2.GetAttributeValue(types.T1));
            Assert.AreEqual(12, g2.GetAttributeValue(types.T2));
        }

        [Test]
        public void ReportMissingAttributes()
        {
            var g1 = new AttributeGroup<types, int>();
            var g2 = new AttributeGroup<types, int>();

            g1.Add(types.T0);
            g1.Add(types.T1, 1);

            g2.Add(types.T0, 10);
            g2.Add(types.T1, 11);
            g2.Add(types.T2, 12, "Tootsie");

            var missing = g1.Apply(g2);
            Assert.AreEqual(1, missing.Count);
            Assert.AreEqual(10, g1.GetAttributeValue(types.T0));
            Assert.AreEqual(12, g1.GetAttributeValue(types.T1));

            missing = g1.Revert(g2);
            Assert.AreEqual(1, missing.Count);
            Assert.AreEqual(0, g1.GetAttributeValue(types.T0));
            Assert.AreEqual(1, g1.GetAttributeValue(types.T1));
        }

        [Test]
        public void CanResetType()
        {
            var g = new AttributeGroup<types, int>();
            g.Add(types.T0, 10);
            g.Add(types.T1, 11);
            g.Add(types.T2, 12, "Tootsie");

            g.Apply(types.T0, 100);
            g.Apply(types.T1, 100);
            g.Apply(types.T2, 100);
            
            Assert.AreEqual(110, g.GetAttributeValue(types.T0));
            Assert.AreEqual(111, g.GetAttributeValue(types.T1));
            Assert.AreEqual(112, g.GetAttributeValue(types.T2));
            
            g.Reset(types.T0, types.T2);
            
            Assert.AreEqual(10, g.GetAttributeValue(types.T0));
            Assert.AreEqual(111, g.GetAttributeValue(types.T1));
            Assert.AreEqual(12, g.GetAttributeValue(types.T2));
            
            g.Reset();
            
            Assert.AreEqual(10, g.GetAttributeValue(types.T0));
            Assert.AreEqual(11, g.GetAttributeValue(types.T1));
            Assert.AreEqual(12, g.GetAttributeValue(types.T2));
        }
        
        [Test]
        public void CanClone()
        {
            var g = new AttributeGroup<types, int> {{types.T0, 10}, {types.T1, 11}, {types.T2, 12, "Tootsie"}};
            var g2 = new AttributeGroup<types, int> {{types.T0, 100}, {types.T1, 100}, {types.T2, 100, "Thrice"}};

            g.Apply(g2);
            
            Assert.AreEqual(110, g.GetAttributeValue(types.T0));
            Assert.AreEqual(111, g.GetAttributeValue(types.T1));
            Assert.AreEqual(112, g.GetAttributeValue(types.T2));

            var clone = (AttributeGroup<types, int>)g.Clone();
            
            Assert.AreEqual(110, clone.GetAttributeValue(types.T0));
            Assert.AreEqual(111, clone.GetAttributeValue(types.T1));
            Assert.AreEqual(112, clone.GetAttributeValue(types.T2));
            
            g.Apply(types.T0, 1000);
            g.Apply(types.T1, 1000);
            g.Apply(types.T2, 1000);
            
            Assert.AreEqual(1110, g.GetAttributeValue(types.T0));
            Assert.AreEqual(1111, g.GetAttributeValue(types.T1));
            Assert.AreEqual(1112, g.GetAttributeValue(types.T2));
            Assert.AreEqual(110, clone.GetAttributeValue(types.T0));
            Assert.AreEqual(111, clone.GetAttributeValue(types.T1));
            Assert.AreEqual(112, clone.GetAttributeValue(types.T2));

            clone.Revert(g2);
            
            Assert.AreEqual(1110, g.GetAttributeValue(types.T0));
            Assert.AreEqual(1111, g.GetAttributeValue(types.T1));
            Assert.AreEqual(1112, g.GetAttributeValue(types.T2));
            Assert.AreEqual(10, clone.GetAttributeValue(types.T0));
            Assert.AreEqual(11, clone.GetAttributeValue(types.T1));
            Assert.AreEqual(12, clone.GetAttributeValue(types.T2));
        }

        [Test]
        public void CanAddCircularModifications()
        {
            var g1 = new AttributeGroup<types, int>();
            var g2 = new AttributeGroup<types, int>();

            g1.Add(types.T0);
            g1.Add(types.T1, 1);
            g1.Add(types.T2, 2, "T-Two");

            g2.Add(types.T0, 10);
            g2.Add(types.T1, 11);
            g2.Add(types.T2, 12, "Tootsie");

            g1.Apply(g2);
            g2.Apply(g1);

            Assert.AreEqual(10, g1.GetAttributeValue(types.T0));
            Assert.AreEqual(12, g1.GetAttributeValue(types.T1));
            Assert.AreEqual(14, g1.GetAttributeValue(types.T2));

            Assert.AreEqual(20, g2.GetAttributeValue(types.T0));
            Assert.AreEqual(23, g2.GetAttributeValue(types.T1));
            Assert.AreEqual(26, g2.GetAttributeValue(types.T2));

            g1.Revert(g2);
            Assert.AreEqual(0, g1.GetAttributeValue(types.T0));
            Assert.AreEqual(1, g1.GetAttributeValue(types.T1));
            Assert.AreEqual(2, g1.GetAttributeValue(types.T2));

            g1 = new AttributeGroup<types, int>();
            g2 = new AttributeGroup<types, int>();

            g1.Add(types.T0);
            g1.Add(types.T1, 1);
            g1.Add(types.T2, 2, "T-Two");

            g2.Add(types.T0, 10);
            g2.Add(types.T1, 11);
            g2.Add(types.T2, 12, "Tootsie");

            g2.Apply(g1);
            g1.Apply(g2);

            Assert.AreEqual(10, g1.GetAttributeValue(types.T0));
            Assert.AreEqual(13, g1.GetAttributeValue(types.T1));
            Assert.AreEqual(16, g1.GetAttributeValue(types.T2));

            Assert.AreEqual(10, g2.GetAttributeValue(types.T0));
            Assert.AreEqual(12, g2.GetAttributeValue(types.T1));
            Assert.AreEqual(14, g2.GetAttributeValue(types.T2));

            g1.Revert(g2);
            Assert.AreEqual(0, g1.GetAttributeValue(types.T0));
            Assert.AreEqual(1, g1.GetAttributeValue(types.T1));
            Assert.AreEqual(2, g1.GetAttributeValue(types.T2));
        }
    }
}