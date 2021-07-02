using static ExoActive.ExoActive<System.Enum, long>;

namespace Example_Effect
{
    public class Actor : Manager.ManagedEntity
    {
        public Actor()
        {
            attributes.Add(HealthAttributes.Health, 100);
            attributes.Add(WeaponAttributes.Power, 10);
        }
    }
}