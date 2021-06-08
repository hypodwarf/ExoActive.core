using System;

namespace ExoActive
{
    public static class TimeTicker
    {
        public static event Action TickEvent;
        public static ulong Ticks { get; private set; }

        public static void AddTicks(ulong tick)
        {
            Ticks += tick;
            TickEvent?.Invoke();
        }
    }
}