using System;

namespace ExoActive
{
    public partial class ExoActive<TKey, TValue>
    {
        public interface IRequirement
        {
            public bool Passes(Entity entity, CapabilityProcessData data);

            public delegate bool Check(Entity entity, CapabilityProcessData data = default);
        }

        public class TraitRequirement : IRequirement
        {
            private readonly Enum Trait;
            private readonly bool RequiredValue;

            private TraitRequirement(Enum trait, bool requiredValue = true)
            {
                Trait = trait;
                RequiredValue = requiredValue;
            }

            public bool Passes(Entity entity, CapabilityProcessData data)
            {
                return entity.Traits.Has(Trait) == RequiredValue;
            }

            public static IRequirement.Check Create(Enum trait, bool requiredValue = true)
            {
                var req = new TraitRequirement(trait, requiredValue);
                return (entity, data) => req.Passes(entity, data);
            }
        }

        public class AttributeRequirement : IRequirement
        {
            public delegate bool Evaluate(TValue value, TValue threshold);

            private readonly TKey attribute;
            private readonly TValue threshold;
            private readonly Evaluate evaluation;

            private AttributeRequirement(TKey attribute, TValue threshold, Evaluate evaluation)
            {
                this.attribute = attribute;
                this.threshold = threshold;
                this.evaluation = evaluation;
            }

            public bool Passes(Entity entity, CapabilityProcessData data)
            {
                return evaluation(entity.Attributes.GetAttributeValue(attribute), threshold);
            }

            public static IRequirement.Check Create(TKey attribute, TValue threshold, Evaluate evaluation)
            {
                var req = new AttributeRequirement(attribute, threshold, evaluation);
                return (entity, data) => req.Passes(entity, data);
            }
        }

        public class StateRequirement<TStateMachine> : IRequirement
            where TStateMachine : EntityStateMachine, new()
        {
            private readonly Enum Trigger;
            private readonly bool Permitted;

            private StateRequirement(Enum trigger, bool permitted = true)
            {
                Trigger = trigger;
                Permitted = permitted;
            }

            public bool Passes(Entity entity, CapabilityProcessData data)
            {
                return entity.IsPermittedTrigger<TStateMachine>(Trigger, data) == Permitted;
            }

            public static IRequirement.Check Create(Enum trigger, bool permitted = true)
            {
                var req = new StateRequirement<TStateMachine>(trigger, permitted);
                return (entity, data) => req.Passes(entity, data);
            }
        }
    }
}