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
                return data.subject.Attributes.Has(HealthAttributes.Health) &&
                       data.actors.All(actor => actor.Attributes.Has(WeaponAttributes.Power));
            }

            public void PerformAction(CapabilityProcessData data)
            {
                var damageDone = data.actors.Aggregate(0L,
                    (power, actor) => (
                        actor.Attributes.GetAttributeValue(HealthAttributes.Health) > 0 ?
                        actor.Attributes.GetAttributeValue(WeaponAttributes.Power) : 0
                        ) + power);

                data.subject.Attributes.AdjustNamedModifier(HealthAttributes.Health, DAMAGE, -damageDone);
            }
        }
        
        public Attack() : base(new ICapabilityProcess[]{}, new ICapabilityProcess[]
        {
            new ReduceHealth()
        })
        {
        }
    }
}