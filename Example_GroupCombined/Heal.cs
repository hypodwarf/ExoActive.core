using System.Linq;
using ExoActive;

namespace Example_GroupCombined
{
    public class Heal : Capability
    {
        private class ImproveHealth : ICapabilityProcess
        {
            private const string HEALING = "Healing";

            private static readonly IRequirement.Check HealerIsAlive =
                AttributeValueRequirement.Create(PhysicalAttributes.Health, DataSelect.Actors, 0,
                    AttributeValueRequirement.Evaluation.GT);

            private static readonly IRequirement.Check HealerHasStrength =
                AttributeRequirement.Create(PhysicalAttributes.Strength, DataSelect.Actors);
            
            private static readonly IRequirement.Check RecieverHasHealth =
                AttributeRequirement.Create(PhysicalAttributes.Health, DataSelect.Targets);
            
            public bool PassesRequirements(CapabilityProcessData data)
            {
                return data.targets.Count == 1 
                       && HealerIsAlive(data)
                       && HealerHasStrength(data)
                       && RecieverHasHealth(data);
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