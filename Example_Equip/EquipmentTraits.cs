using System;

namespace Example_Equip
{
    [Flags]
    public enum EquipmentTraits
    {
        Ring = 1 << 0,
        Belt = 1 << 1,
        Equip = 1 << 2,
    }
}