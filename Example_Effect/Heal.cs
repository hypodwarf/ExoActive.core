using System.Linq;
using static ExoActive.ExoActive<System.Enum, long>;

namespace Example_Effect
{
    public class Heal : Capability
    {
        private class ImproveHealth : ICapabilityProcess
        {
            private const string HEALING = "Healing";
            public bool PassesRequirements(CapabilityProcessData data)
            {
                return data.subject.Attributes.Has(WeaponAttributes.Power)
                    && data.targets.All(actor => actor.Attributes.Has(HealthAttributes.Health));
            }

            public void PerformAction(CapabilityProcessData data)
            {
                var power = data.subject.Attributes.GetAttributeValue(WeaponAttributes.Power);
                data.targets.ForEach(target => target.Attributes.AdjustNamedModifier(HealthAttributes.Health, HEALING, power));
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