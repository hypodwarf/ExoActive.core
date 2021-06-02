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
        public void CanHasOnEmpty()
        {
            var c = new Characteristics();
            Assert.False(c.Has(Tests.TestFlag.Min));
        }
    }
}