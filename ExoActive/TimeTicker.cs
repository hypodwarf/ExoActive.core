using System;
using System.Runtime.Serialization;

namespace ExoActive
{
    [DataContract]
    public static class TimeTicker
    {
        public static event Action TickEvent;

        [DataMember] public static ulong Ticks { get; private set; }

        public static void AddTicks(ulong tick)
        {
            Ticks += tick;
            TickEvent?.Invoke();
        }
    }
}