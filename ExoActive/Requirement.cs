using System;
using System.Linq;

namespace ExoActive
{
    public interface IRequirement
    {
        public bool Passes(CapabilityProcessData data);

        public delegate bool Check(CapabilityProcessData data = default);
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

        public bool Passes(CapabilityProcessData data)
        {
            return data.actors.All(actor => actor.Traits.Has(Trait) == RequiredValue);
        }

        public static IRequirement.Check Create(Enum trait, bool requiredValue = true)
        {
            var req = new TraitRequirement(trait, requiredValue);
            return data => req.Passes(data);
        }
    }

    public class AttributeRequirement : IRequirement
    {
        public delegate bool Evaluate(long value, long threshold);

        private readonly Enum attribute;
        private readonly long threshold;
        private readonly Evaluate evaluation;

        private AttributeRequirement(Enum attribute, long threshold, Evaluate evaluation)
        {
            this.attribute = attribute;
            this.threshold = threshold;
            this.evaluation = evaluation;
        }

        public bool Passes(CapabilityProcessData data)
        {
            return data.actors.All(actor => evaluation(actor.Attributes.GetAttributeValue(attribute), threshold));
        }

        public static IRequirement.Check Create(Enum attribute, long threshold, Evaluate evaluation)
        {
            var req = new AttributeRequirement(attribute, threshold, evaluation);
            return data => req.Passes(data);
        }
    }

    public class StateRequirement<TStateMachine> : IRequirement
        where TStateMachine : EntityStateMachine, new()
    {
        private readonly Enum Trigger;
        private readonly bool Permitted;
        private readonly DataSelect Selector;

        private StateRequirement(Enum trigger, DataSelect selector, bool permitted = true)
        {
            Trigger = trigger;
            Permitted = permitted;
            Selector = selector;
        }

        public bool Passes(CapabilityProcessData data)
        {
            bool passes = true;
            if (Selector.HasFlag(DataSelect.Actors))
            {
                passes = data.actors.All(actor => actor.IsPermittedTrigger<TStateMachine>(Trigger, data) == Permitted);
            }
            
            if (Selector.HasFlag(DataSelect.Targets))
            {
                passes = passes && data.targets.All(target => target.IsPermittedTrigger<TStateMachine>(Trigger, data) == Permitted);
            }

            return passes;
        }

        public static IRequirement.Check Create(Enum trigger, DataSelect selector, bool permitted = true)
        {
            var req = new StateRequirement<TStateMachine>(trigger, selector, permitted);
            return data => req.Passes(data);
        }
    }
}