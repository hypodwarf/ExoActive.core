using System;
using ExoActive;
using NUnit.Framework;

namespace Tests
{
    [Flags] 
    public enum TestFlag : ulong
    {
        None = 0,
        Min = 1UL << 0,
        Max = 1UL << 63
    }

    public class CharacteristicsTest
    {
        [Flags]
        public enum TestFlag : ulong
        {
            None = 0,
            Min = 1UL << 0,
            Mid = 1UL << 31,
            Max = 1UL << 63
        }
        
        [Test]
        public void CanCreate()
        {
            var c = new Characteristics();
            Assert.Pass();
        }
        
        [Test]
        public void CanAdd()
        {
            var c = new Characteristics();
            c.Add(TestFlag.Min | TestFlag.Max);

            Assert.True(c.Has(TestFlag.Min));
            Assert.True(c.Has(TestFlag.Max));
            Assert.True(c.Has(TestFlag.Min | TestFlag.Max));
            
            Assert.False(c.Has(Tests.TestFlag.Min));
            Assert.False(c.Has(Tests.TestFlag.Max));
            Assert.False(c.Has(Tests.TestFlag.Min | Tests.TestFlag.Max));
            
            c = new Characteristics();
            c.Add(TestFlag.Min);

            Assert.True(c.Has(TestFlag.Min));
            Assert.False(c.Has(TestFlag.Max));
            Assert.False(c.Has(TestFlag.Min | TestFlag.Max));
            
            Assert.False(c.Has(Tests.TestFlag.Min));
            Assert.False(c.Has(Tests.TestFlag.Max));
            Assert.False(c.Has(Tests.TestFlag.Min | Tests.TestFlag.Max));
        }
        
        [Test]
        public void CanRemove()
        {
            var c = new Characteristics();
            c.Add(TestFlag.Min | TestFlag.Max);

            Assert.True(c.Has(TestFlag.Min));
            Assert.True(c.Has(TestFlag.Max));
            Assert.True(c.Has(TestFlag.Min | TestFlag.Max));
            
            c.Remove(Tests.TestFlag.Min);
            
            Assert.True(c.Has(TestFlag.Min));
            
            c.Remove(TestFlag.Min);
            
            Assert.False(c.Has(TestFlag.Min));
            Assert.True(c.Has(TestFlag.Max));
            Assert.False(c.Has(TestFlag.Min | TestFlag.Max));
        }

        [Test]
        public void HasWhenEmpty()
        {
            var c = new Characteristics();
            Assert.False(c.Has(Tests.TestFlag.Min));
        }
        
        [Test]
        public void HandlingUnions()
        {
            var c = new Characteristics();
            c.Add(TestFlag.Min | TestFlag.Mid | TestFlag.Max);
            c.Add(TestFlag.Min | TestFlag.Mid);
            c.Add(TestFlag.Mid | TestFlag.Max);
            c.Add(TestFlag.Min | TestFlag.Max);
            c.Add(TestFlag.Min);
            c.Add(TestFlag.Mid);
            c.Add(TestFlag.Max);
            
            Assert.True(c.Has(TestFlag.Min));
            Assert.True(c.Has(TestFlag.Mid));
            Assert.True(c.Has(TestFlag.Max));
            
            c.Remove(TestFlag.Min | TestFlag.Mid | TestFlag.Max);
            
            Assert.False(c.Has(TestFlag.Min));
            Assert.False(c.Has(TestFlag.Mid));
            Assert.False(c.Has(TestFlag.Max));
        }

        [Test]
        public void Values()
        {
            var c = new Characteristics();
            
            Assert.AreEqual(0, c.Value<TestFlag>());
            c.Add(TestFlag.Min);
            Assert.AreEqual(1UL << 0, c.Value<TestFlag>());
            c.Add(TestFlag.Mid);
            Assert.AreEqual(1UL << 0 | 1UL << 31, c.Value<TestFlag>());
            c.Add(TestFlag.Max);
            Assert.AreEqual(1UL << 0 | 1UL << 31 | 1UL << 63, c.Value<TestFlag>());
            Assert.AreEqual(1UL << 0 | 1UL << 31 | 1UL << 63, c.Value(typeof(TestFlag)));
        }
    }
}