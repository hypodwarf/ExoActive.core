using System;
using static ExoActive.ExoActive<System.Enum, int>;

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
    }
}