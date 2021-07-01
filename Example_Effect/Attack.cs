using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using static ExoActive.Type<System.Enum, int>;

namespace Example_Effect
{
    public class Attack: Capability
    {
        private class ReduceHealth : ICapabilityProcess
        {
            public bool PassesRequirements(CapabilityProcessData data)
            {
                return data.subject.Attributes.Has(HealthAttributes.Health) &&
                       data.actors.All(actor => actor.Attributes.Has(WeaponAttributes.Power));
            }

            public void PerformAction(CapabilityProcessData data)
            {
                data.subject.Attributes.Apply(HealthAttributes.Health,
                    data.actors.Aggregate(0,
                        (power, actor) => power - actor.Attributes.GetAttributeValue(WeaponAttributes.Power)));
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