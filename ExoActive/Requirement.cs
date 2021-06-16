using System;

namespace ExoActive
{
    public interface IRequirement
    {
        public bool Passes(Entity entity);

        public delegate bool Check(Entity entity);
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

        public bool Passes(Entity entity)
        {
            return entity.Traits.Has(Trait) == RequiredValue;
        }

        public static IRequirement.Check Create(Enum trait, bool requiredValue = true)
        {
            var req = new TraitRequirement(trait, requiredValue);
            return entity => req.Passes(entity);
        }
    }

    public class AttributeRequirement : IRequirement
    {
        public delegate bool Evaluate(int value, int threshold);

        private readonly Enum Attribute;
        private readonly int Threshold;
        private readonly Evaluate Evaluation;

        private AttributeRequirement(Enum attribute, int threshold, Evaluate evaluation)
        {
            Attribute = attribute;
            Threshold = threshold;
            Evaluation = evaluation;
        }

        public bool Passes(Entity entity)
        {
            return Evaluation(entity.Attributes.GetAttributeValue(Attribute), Threshold);
        }

        public static IRequirement.Check Create(Enum attribute, int threshold, Evaluate evaluation)
        {
            var req = new AttributeRequirement(attribute, threshold, evaluation);
            return entity => req.Passes(entity);
        }
    }

    public class StateRequirement<S> : IRequirement where S : State, new()
    {
        private readonly Enum Trigger;
        private readonly bool Permitted;

        private StateRequirement(Enum trigger, bool permitted = true)
        {
            Trigger = trigger;
            Permitted = permitted;
        }

        public bool Passes(Entity entity)
        {
            return entity.IsPermittedTrigger<S>(Trigger) == Permitted;
        }

        public static IRequirement.Check Create(Enum trigger, bool permitted = true)
        {
            var req = new StateRequirement<S>(trigger, permitted);
            return entity => req.Passes(entity);
        }
    }
}