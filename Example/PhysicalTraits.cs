using System;

namespace Example
{
    [Flags]
    public enum PhysicalTraits : ulong
    {
        CanCarry = 1UL << 0,
        Carriable = 1UL << 1
    }
}