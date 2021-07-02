using System;
using static ExoActive.ExoActive<System.Enum, int>;
using NUnit.Framework;

namespace Tests
{
    [Flags]
    public enum TestFlag : ulong
    {
        // None = 0,
        Min = 1UL << 0,
        Max = 1UL << 63
    }

    public class TraitsTest
    {
        [Flags]
        public enum TestFlag : ulong
        {
            // None = 0,
            Min = 1UL << 0,
            Mid = 1UL << 31,
            Max = 1UL << 63
        }

        [Test]
        public void CanCreate()
        {
            var c = new Traits();
            Assert.Pass();
        }

        [Test]
        public void CanAdd()
        {
            var c = new Traits();
            c.Add(TestFlag.Min | TestFlag.Max);

            Assert.True(c.Has(TestFlag.Min));
            Assert.True(c.Has(TestFlag.Max));
            Assert.True(c.Has(TestFlag.Min | TestFlag.Max));

            Assert.False(c.Has(Tests.TestFlag.Min));
            Assert.False(c.Has(Tests.TestFlag.Max));
            Assert.False(c.Has(Tests.TestFlag.Min | Tests.TestFlag.Max));

            c = new Traits();
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
            var c = new Traits();
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
            var c = new Traits();
            Assert.False(c.Has(Tests.TestFlag.Min));
        }

        [Test]
        public void HandlingUnions()
        {
            var c = new Traits();
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
            var c = new Traits();

            Assert.AreEqual(Enum.ToObject(typeof(TestFlag),0), c.Value<TestFlag>());
            c.Add(TestFlag.Min);
            Assert.AreEqual(TestFlag.Min, c.Value<TestFlag>());
            c.Add(TestFlag.Mid);
            Assert.AreEqual(TestFlag.Min | TestFlag.Mid, c.Value<TestFlag>());
            c.Add(TestFlag.Max);
            Assert.AreEqual(TestFlag.Min | TestFlag.Mid | TestFlag.Max, c.Value<TestFlag>());
            Assert.AreEqual(TestFlag.Min | TestFlag.Mid | TestFlag.Max, c.Value(typeof(TestFlag)));
        }
    }
}