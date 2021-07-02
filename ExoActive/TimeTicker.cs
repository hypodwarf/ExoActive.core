using System;
using System.Runtime.Serialization;

namespace ExoActive
{
    public static partial class Type<TKey, TValue>
    {
        [DataContract]
        public static class TimeTicker
        {
            public static event Action TickEvent;

            // ReSharper disable once StaticMemberInGenericType
            [DataMember] public static ulong Ticks { get; private set; }

            public static void AddTicks(ulong tick)
            {
                Ticks += tick;
                TickEvent?.Invoke();
            }
        }
    }
}