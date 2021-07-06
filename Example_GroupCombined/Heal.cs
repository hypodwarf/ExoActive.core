using System.Linq;
using ExoActive;

namespace Example_GroupCombined
{
    public class Heal : Capability
    {
        private class ImproveHealth : ICapabilityProcess
        {
            private const string HEALING = "Healing";
            public bool PassesRequirements(CapabilityProcessData data)
            {
                return data.subject.Attributes.GetAttributeValue(PhysicalAttributes.Health) > 0
                       && data.subject.Attributes.Has(PhysicalAttributes.Strength)
                       && data.targets.Count == 1
                       && data.targets[0].Attributes.Has(PhysicalAttributes.Health);
            }

            public void PerformAction(CapabilityProcessData data)
            {
                var power = data.subject.Attributes.GetAttributeValue(PhysicalAttributes.Strength);
                data.targets.ForEach(target => target.Attributes.AdjustNamedModifier(PhysicalAttributes.Health, HEALING, power));
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