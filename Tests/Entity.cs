using System;
using ExoActive;
using NUnit.Framework;

namespace Tests
{
    public class TestEntity : Entity
    {
        [Flags]
        public enum EntityTraits : ulong
        {
            Happy = 1UL << 0,
            Sad = 1UL << 1
        }

        public enum EntityAttributes
        {
            Strength
        }

        public TestEntity()
        {
            traits.Add(EntityTraits.Happy);
            attributes.Add(EntityAttributes.Strength, 10);
        }
    }

    public class EntityTest
    {
        [Test]
        public void CanGetAttributeValues()
        {
            var E = new TestEntity();
            Assert.AreEqual(10L, E.Attributes.GetAttributeValue(TestEntity.EntityAttributes.Strength));
            // data.subject.Attributes.GetAttributeValue(target.Traits.Value<EquipmentTraits>() | EquipmentTraits.Equip);
        }

        [Test]
        public void CanHandleInvalidAttriutes()
        {
            var E = new TestEntity();
            Assert.AreEqual(-1L, E.Attributes.GetAttributeValue(TestEntity.EntityTraits.Happy, -1L));
        }
    }
}