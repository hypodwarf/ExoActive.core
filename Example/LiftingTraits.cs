using System;

namespace Example
{
    [Flags]
    public enum LiftingTraits : ulong
    {
        Lifteable = 1UL << 0,
        CanLift = 1UL << 1
    }
}