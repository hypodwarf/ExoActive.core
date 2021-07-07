using System.Linq;
using ExoActive;

namespace Example_Effect
{
    public class Heal : Capability
    {
        private class ImproveHealth : ICapabilityProcess
        {
            private const string HEALING = "Healing";
            public bool PassesRequirements(CapabilityProcessData data)
            {
                return data.actors.All(healer => 
                           healer.Attributes.Has(WeaponAttributes.Power) 
                           && healer.Attributes.GetAttributeValue(HealthAttributes.Health) > 0 )
                       && data.targets.All(receiver => receiver.Attributes.Has(HealthAttributes.Health));
            }

            public void PerformAction(CapabilityProcessData data)
            {
                var power = data.actors.Aggregate(0L, (acc, healer) => acc + healer.Attributes.GetAttributeValue(WeaponAttributes.Power));
                data.targets.ForEach(receiver => receiver.Attributes.AdjustNamedModifier(HealthAttributes.Health, HEALING, power));
            }
        }

        public Heal() : base(new ICapabilityProcess[]
        {
            new ImproveHealth()
        }, new ICapabilityProcess[]{})
        {
        }
    }
}