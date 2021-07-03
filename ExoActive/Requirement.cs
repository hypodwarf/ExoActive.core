using System;

namespace ExoActive
{
    public interface IRequirement
    {
        public bool Passes(IEntity entity, CapabilityProcessData data);

        public delegate bool Check(IEntity entity, CapabilityProcessData data = default);
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

        public bool Passes(IEntity entity, CapabilityProcessData data)
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
        public delegate bool Evaluate(long value, long threshold);

        private readonly System.Enum attribute;
        private readonly long threshold;
        private readonly Evaluate evaluation;

        private AttributeRequirement(System.Enum attribute, long threshold, Evaluate evaluation)
        {
            this.attribute = attribute;
            this.threshold = threshold;
            this.evaluation = evaluation;
        }

        public bool Passes(IEntity entity, CapabilityProcessData data)
        {
            return evaluation(entity.Attributes.GetAttributeValue(attribute), threshold);
        }

        public static IRequirement.Check Create(System.Enum attribute, long threshold, Evaluate evaluation)
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

        public bool Passes(IEntity entity, CapabilityProcessData data)
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