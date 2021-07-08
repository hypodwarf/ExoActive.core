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
                return data.actors.All(healer => 
                           healer.Attributes.GetAttributeValue(PhysicalAttributes.Health) > 0 
                           && healer.Attributes.Has(PhysicalAttributes.Strength)
                       )
                       && data.targets.Count == 1
                       && data.targets.All(receiver => receiver.Attributes.Has(PhysicalAttributes.Health));
            }

            public void PerformAction(CapabilityProcessData data)
            {
                var power = data.actors.Sum(healer => healer.Attributes.GetAttributeValue(PhysicalAttributes.Strength));
                data.targets.ForEach(receiver => receiver.Attributes.AdjustNamedModifier(PhysicalAttributes.Health, HEALING, power));
            }
        }

        public Heal() : base(new ImproveHealth())
        {
        }
    }
}