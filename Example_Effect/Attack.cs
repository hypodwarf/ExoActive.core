using System.Linq;
using ExoActive;

namespace Example_Effect
{
    public class Attack: Capability
    {
        private class ReduceHealth : ICapabilityProcess
        {
            private const string DAMAGE = "Damage";
            public bool PassesRequirements(CapabilityProcessData data)
            {
                return data.actors.All(victim => victim.Attributes.Has(HealthAttributes.Health)) &&
                       data.targets.All(attacker => attacker.Attributes.Has(WeaponAttributes.Power));
            }

            public void PerformAction(CapabilityProcessData data)
            {
                var damageDone = data.actors.Sum(attacker => 
                    attacker.Attributes.GetAttributeValue(HealthAttributes.Health) > 0 ? 
                        attacker.Attributes.GetAttributeValue(WeaponAttributes.Power) : 0
                );

                data.targets.ForEach(victim => victim.Attributes.AdjustNamedModifier(HealthAttributes.Health, DAMAGE, -damageDone));
            }
        }
        
        public Attack() : base(new ReduceHealth())
        {
        }
    }
}