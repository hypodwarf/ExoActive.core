using System.Linq;
using static ExoActive.Type<System.Enum, int>;

namespace Example_Effect
{
    public class Heal : Capability
    {
        private class ImproveHealth : ICapabilityProcess
        {
            public bool PassesRequirements(CapabilityProcessData data)
            {
                return data.subject.Attributes.Has(WeaponAttributes.Power)
                    && data.targets.All(actor => actor.Attributes.Has(HealthAttributes.Health));
            }

            public void PerformAction(CapabilityProcessData data)
            {
                var power = data.subject.Attributes.GetAttributeValue(WeaponAttributes.Power);
                data.targets.ForEach(target => target.Attributes.Apply(HealthAttributes.Health, power));
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