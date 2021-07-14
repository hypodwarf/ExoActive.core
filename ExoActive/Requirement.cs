using System;
using System.Linq;

namespace ExoActive
{
    public interface IRequirement
    {
        public bool Passes(CapabilityProcessData data);

        public bool Passes(IEntity entity, CapabilityProcessData data);

        public delegate bool Check(CapabilityProcessData data = default);
    }

    public abstract class Requirement : IRequirement
    {
        private readonly DataSelect Selector;

        protected Requirement(DataSelect selector)
        {
            Selector = selector;
        }

        public bool Passes(CapabilityProcessData data)
        {
            bool passes = true;
            if (Selector.HasFlag(DataSelect.Actors))
            {
                passes = data.actors.All(actor => Passes(actor, data));
            }
            
            if (Selector.HasFlag(DataSelect.Targets))
            {
                passes = passes && data.targets.All(target => Passes(target, data));
            }

            return passes;
        }

        public abstract bool Passes(IEntity entity, CapabilityProcessData data);
    }

    public class TraitRequirement : Requirement
    {
        private readonly Enum Trait;
        private readonly bool RequiredValue;

        private TraitRequirement(Enum trait, DataSelect selector, bool requiredValue = true) : base(selector)
        {
            Trait = trait;
            RequiredValue = requiredValue;
        }

        public static IRequirement.Check Create(Enum trait, DataSelect selector, bool requiredValue = true)
        {
            var req = new TraitRequirement(trait, selector, requiredValue);
            return data => req.Passes(data);
        }

        public override bool Passes(IEntity entity, CapabilityProcessData data) => entity.Traits.Has(Trait) == RequiredValue;
    }

    public class AttributeRequirement : Requirement
    {
        public delegate bool Evaluate(long value, long threshold);

        private readonly Enum attribute;
        private readonly long threshold;
        private readonly Evaluate evaluation;

        private AttributeRequirement(Enum attribute, DataSelect selector, long threshold, Evaluate evaluation) : base(selector)
        {
            this.attribute = attribute;
            this.threshold = threshold;
            this.evaluation = evaluation;
        }

        public override bool Passes(IEntity entity, CapabilityProcessData data) =>
            evaluation(entity.Attributes.GetAttributeValue(attribute), threshold);

        public static IRequirement.Check Create(Enum attribute, DataSelect selector, long threshold, Evaluate evaluation)
        {
            var req = new AttributeRequirement(attribute, selector, threshold, evaluation);
            return data => req.Passes(data);
        }
    }

    public class StateRequirement<TStateMachine> : Requirement
        where TStateMachine : EntityStateMachine, new()
    {
        private readonly Enum Trigger;
        private readonly bool Permitted;

        private StateRequirement(Enum trigger, DataSelect selector, bool permitted = true) : base(selector)
        {
            Trigger = trigger;
            Permitted = permitted;
        }

        public override bool Passes(IEntity entity, CapabilityProcessData data) =>
            entity.IsPermittedTrigger<TStateMachine>(Trigger, data) == Permitted;

        public static IRequirement.Check Create(Enum trigger, DataSelect selector, bool permitted = true)
        {
            var req = new StateRequirement<TStateMachine>(trigger, selector, permitted);
            return data => req.Passes(data);
        }
    }
}