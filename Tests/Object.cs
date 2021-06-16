using System;
using Object = ExoActive.Object;

namespace Tests
{
    public class TestObj : Object
    {
        [Flags]
        public enum ObjChar : ulong
        {
            Happy = 1UL << 0,
            Sad = 1UL << 1
        }

        public enum ObjAttr
        {
            Strength
        }

        public TestObj()
        {
            traits.Add(ObjChar.Happy);
            attributes.Add(ObjAttr.Strength, 10);
        }
    }

    public class ObjectTest
    {
    }
}