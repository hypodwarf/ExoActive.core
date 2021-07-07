using System.Linq;
using ExoActive;

namespace Example_GroupCombined
{
    public class Attack: Capability
    {
        private class HitTarget : ICapabilityProcess
        {
            private const string DAMAGE = "Damage";
            public bool PassesRequirements(CapabilityProcessData data)
            {
                return data.actors.All(actor => 
                           actor.Attributes.GetAttributeValue(PhysicalAttributes.Health) > 0 
                           && actor.Attributes.Has(PhysicalAttributes.Strength))
                       && data.targets.Count == 1
                       && data.targets.All(target => target.Attributes.Has(PhysicalAttributes.Health));
            }

            public void PerformAction(CapabilityProcessData data)
            {
                var damageDone = data.actors.Sum(actor => actor.Attributes.GetAttributeValue(PhysicalAttributes.Strength));
                data.targets.ForEach(target => target.Attributes.AdjustNamedModifier(PhysicalAttributes.Health, DAMAGE, -damageDone));
            }
        }
        
        public Attack() : base(new ICapabilityProcess[]{
            new HitTarget()
        })
        {
        }
    }
}