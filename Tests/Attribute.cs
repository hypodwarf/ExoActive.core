using System;
using ExoActive;
using NUnit.Framework;

namespace Tests
{
    public class AttributeTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void CanCreate()
        {
            var x = new Attribute<float>("Jason", 1);
            var y = new Attribute<string>("Rohman", "Hi");
            var z = new Attribute<Exception>("Exception", new Exception());
            Assert.Pass();
        }

        [Test]
        public void CanInsertMultipleBranches()
        {
            var a = new Attribute<float>("a", 1);
            var u = new Attribute<float>("u", 1);
            var v = new Attribute<float>("v", 1);
            var w = new Attribute<float>("w", 1);
            var x = new Attribute<float>("x", 1);
            var y = new Attribute<float>("y", 1);
            var z = new Attribute<float>("z", 1);

            var v1 = v.InsertModifier(u);
            var w1 = w.InsertModifier(v1);
            var x1 = x.InsertModifier(w1);
            var y1 = y.InsertModifier(x1);
            var z1 = z.InsertModifier(y1);

            var root1 = a
                .InsertModifier(z1)
                .InsertModifier(y1)
                .InsertModifier(x1)
                .InsertModifier(w1)
                .InsertModifier(v1)
                .InsertModifier(u);

            // AttributeHelper.PrintAttributeTree(root1);

            Assert.AreEqual(22f, root1.modifiedValue.value);
            Assert.AreEqual(6, root1.version);


            var v2 = v.InsertModifier(u);
            var w2 = w.InsertModifier(v2);
            var y2 = y.InsertModifier(x);
            var z2 = z.InsertModifier(y2);
            var root2 = a
                .InsertModifier(w2)
                .InsertModifier(z2);

            Assert.AreEqual(3f, w2.modifiedValue.value);
            Assert.AreEqual(3f, z2.modifiedValue.value);
            Assert.AreEqual(7f, root2.modifiedValue.value);
            Assert.AreEqual(2, root2.version);

            // AttributeHelper.PrintAttributeTree(root2);
        }

        [Test]
        public void CanInsertDuplicateChildren()
        {
            var x = new Attribute<float>("x", 1);
            var y = new Attribute<float>("y", 1);
            var z = new Attribute<float>("z", 1);

            var y1 = y.InsertModifier(x);
            var z1 = z.InsertModifier(y);

            Assert.AreNotEqual(z1, z1.InsertModifier(y));
            Assert.AreNotEqual(z1, z1.InsertModifier(y1));
        }
        
        [Test]
        public void CannotInsertDuplicateChildren()
        {
            var x = new Attribute<float>("x", 1);
            var y = new Attribute<float>("y", 1);
            var z = new Attribute<float>("z", 1);

            var y1 = y.InsertModifier(x);
            var z1 = z.InsertModifier(y);

            Assert.AreEqual(z1, z1.InsertModifier(y));
            Assert.AreEqual(z1, z1.InsertModifier(y1));
        }

        [Test]
        public void CanInsertSelfAsChild()
        {
            var z = new Attribute<float>("z", 1);
            var z1 = z.InsertModifier(z);

            Assert.AreEqual(2f, z1.modifiedValue.value);

            var z2 = z1.InsertModifier(z);
            // This doesn't change the attr
            Assert.AreEqual(z1, z2);

            var z3 = z.InsertModifier(z1);
            // This does change the attr
            Assert.AreNotEqual(z1, z3);

            // This demonstrates a lack of the commutative property which is not intuative. 
        }

        [Test]
        public void CannotInsertSelfAsChild()
        {
            var z = new Attribute<float>("z", 1);
            var z1 = z.InsertModifier(z);

            Assert.AreEqual(1f, z1.modifiedValue.value);

            var z2 = z1.InsertModifier(z);
            Assert.AreEqual(z1, z2);

            var z3 = z.InsertModifier(z1);
            Assert.AreEqual(z1, z3);
        }
    }
}