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
        private readonly DataSelect selector;

        protected Requirement(DataSelect selector)
        {
            this.selector = selector;
        }

        public bool Passes(CapabilityProcessData data)
        {
            bool passes = true;
            if (selector.HasFlag(DataSelect.Actors))
            {
                passes = data.actors.All(actor => Passes(actor, data));
            }
            
            if (selector.HasFlag(DataSelect.Targets))
            {
                passes = passes && data.targets.All(target => Passes(target, data));
            }

            return passes;
        }

        public abstract bool Passes(IEntity entity, CapabilityProcessData data);
    }

    public class TraitRequirement : Requirement
    {
        private readonly Enum trait;
        private readonly bool hasTrait;

        private TraitRequirement(Enum trait, DataSelect selector, bool hasTrait = true) : base(selector)
        {
            this.trait = trait;
            this.hasTrait = hasTrait;
        }

        public static IRequirement.Check Create(Enum trait, DataSelect selector, bool hasTrait = true)
        {
            var req = new TraitRequirement(trait, selector, hasTrait);
            return data => req.Passes(data);
        }

        public override bool Passes(IEntity entity, CapabilityProcessData data) => entity.Traits.Has(trait) == hasTrait;
    }
    
    public class AttributeRequirement : Requirement
    {
        private readonly Enum attribute;
        private readonly bool hasAttribute;

        private AttributeRequirement(Enum attribute, DataSelect selector, bool hasAttribute = true) : base(selector)
        {
            this.attribute = attribute;
            this.hasAttribute = hasAttribute;
        }

        public static IRequirement.Check Create(Enum trait, DataSelect selector, bool hasAttribute = true)
        {
            var req = new AttributeRequirement(trait, selector, hasAttribute);
            return data => req.Passes(data);
        }

        public override bool Passes(IEntity entity, CapabilityProcessData data) => entity.Attributes.Has(attribute) == hasAttribute;
    }

    public class AttributeValueRequirement : Requirement
    {
        public delegate bool Evaluate(long value, long threshold);

        private readonly Enum attribute;
        private readonly long threshold;
        private readonly Evaluate evaluation;

        public enum Evaluation
        {
            EQ,
            NEQ,
            GT,
            GTE,
            LT,
            LTE,
        }

        private AttributeValueRequirement(Enum attribute, DataSelect selector, long threshold, Evaluate evaluation) : base(selector)
        {
            this.attribute = attribute;
            this.threshold = threshold;
            this.evaluation = evaluation;
        }

        public override bool Passes(IEntity entity, CapabilityProcessData data) =>
            evaluation(entity.Attributes.GetAttributeValue(attribute), threshold);

        public static IRequirement.Check Create(Enum attribute, DataSelect selector, long threshold, Evaluate evaluation)
        {
            var req = new AttributeValueRequirement(attribute, selector, threshold, evaluation);
            return data => req.Passes(data);
        }
        
        public static IRequirement.Check Create(Enum attribute, DataSelect selector, long threshold, Evaluation evaluation)
        {
            Evaluate evaluate = evaluation switch
            {
                Evaluation.EQ => (value, th) => value == th,
                Evaluation.NEQ => (value, th) => value != th,
                Evaluation.GT => (value, th) => value > th,
                Evaluation.GTE => (value, th) => value >= th,
                Evaluation.LT => (value, th) => value < th,
                Evaluation.LTE => (value, th) => value <= th,
                _ => throw new ArgumentOutOfRangeException(nameof(evaluation), evaluation, null)
            };
            return Create(attribute, selector, threshold, evaluate);
        }
    }

    public class StateRequirement<TStateMachine> : Requirement
        where TStateMachine : EntityStateMachine, new()
    {
        private readonly Enum trigger;
        private readonly bool permitted;

        private StateRequirement(Enum trigger, DataSelect selector, bool permitted = true) : base(selector)
        {
            this.trigger = trigger;
            this.permitted = permitted;
        }

        public override bool Passes(IEntity entity, CapabilityProcessData data) =>
            entity.IsPermittedTrigger<TStateMachine>(trigger, data) == permitted;

        public static IRequirement.Check Create(Enum trigger, DataSelect selector, bool permitted = true)
        {
            var req = new StateRequirement<TStateMachine>(trigger, selector, permitted);
            return data => req.Passes(data);
        }
    }
}