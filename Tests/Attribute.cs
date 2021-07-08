using NUnit.Framework;

namespace Tests
{
    public class AttributeTests
    {
        [Test]
        public void CanCreate()
        {
            var _ = new ExoActive.Attribute("Jason", 1);
            Assert.Pass();
        }

        [Test]
        public void CanInsertMultipleBranches()
        {
            var a = new ExoActive.Attribute("a", 1);
            var u = new ExoActive.Attribute("u", 1);
            var v = new ExoActive.Attribute("v", 1);
            var w = new ExoActive.Attribute("w", 1);
            var x = new ExoActive.Attribute("x", 1);
            var y = new ExoActive.Attribute("y", 1);
            var z = new ExoActive.Attribute("z", 1);

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
        public void CannotInsertDuplicateChild()
        {
            var x = new ExoActive.Attribute("x", 1);
            var y = new ExoActive.Attribute("y", 1);
            var z = new ExoActive.Attribute("z", 1);

            var y1 = y.InsertModifier(x);
            var z1 = z.InsertModifier(y);

            Assert.AreEqual(z1, z1.InsertModifier(y));
            Assert.AreEqual(z1, z1.InsertModifier(y1));
        }

        [Test]
        public void CanInsertSelfAsChild()
        {
            var z = new ExoActive.Attribute("z", 1);
            var z1 = z.InsertModifier(z);

            Assert.AreEqual(2f, z1.modifiedValue.value);

            var z2 = z1.InsertModifier(z);
            // This doesn't change the attr because z1 already has a 'z' child
            Assert.AreEqual(z1, z2);

            var z3 = z.InsertModifier(z1);
            // This does change the attr
            Assert.AreNotEqual(z1, z3);
        }

        [Test]
        public void CanRemoveChild()
        {
            var a = new ExoActive.Attribute("a", 1);
            var u = new ExoActive.Attribute("u", 2);
            var v = new ExoActive.Attribute("v", 1);
            var w = new ExoActive.Attribute("w", 1);
            var x = new ExoActive.Attribute("x", 3);
            var y = new ExoActive.Attribute("y", 1);
            var z = new ExoActive.Attribute("z", 1);

            var v2 = v.InsertModifier(u);
            var w2 = w.InsertModifier(v2);
            var y2 = y.InsertModifier(x);
            var z2 = z.InsertModifier(y2);
            var root1 = a
                .InsertModifier(w2)
                .InsertModifier(z2);

            Assert.AreEqual(10f, root1.modifiedValue.value);

            var root2 = root1.RemoveModifier(w2);
            Assert.AreEqual(6f, root2.modifiedValue.value);

            var root3 = root1.RemoveModifier(z2);
            Assert.AreEqual(5f, root3.modifiedValue.value);
        }

        [Test]
        public void CanReset()
        {
            var x = new ExoActive.Attribute("a", 1);
            var y = new ExoActive.Attribute("u", 2);
            var z = new ExoActive.Attribute("v", 3);
            
            var x2 = x.InsertModifier(y);
            var x3 = x2.InsertModifier(z);
            
            Assert.AreEqual(6f, x3.modifiedValue.value);

            var x4 = x3.Reset();
            Assert.AreEqual(x.modifiedValue.value, x4.modifiedValue.value);
        }

        [Test]
        public void CanUpdateChild()
        {
            var a = new ExoActive.Attribute("a", 1);
            var u = new ExoActive.Attribute("u", 2);
            var v = new ExoActive.Attribute("v", 1);
            var w = new ExoActive.Attribute("w", 1);
            var x = new ExoActive.Attribute("x", 3);
            var y = new ExoActive.Attribute("y", 1);
            var z = new ExoActive.Attribute("z", 1);

            var v2 = v.InsertModifier(u);
            var w2 = w.InsertModifier(v2);
            var y2 = y.InsertModifier(x);
            var z2 = z.InsertModifier(y2);
            var root1 = a
                .InsertModifier(w2)
                .InsertModifier(z2);

            Assert.AreEqual(10f, root1.modifiedValue.value);

            var w3 = w.InsertModifier(v);
            var root2 = root1.UpdateModifier(w3);
            Assert.AreEqual(8f, root2.modifiedValue.value);

            var z3 = z.InsertModifier(y);
            var root3 = root1.UpdateModifier(z3);
            Assert.AreEqual(7f, root3.modifiedValue.value);
        }
    }
}