using System;

namespace ExoActive
{
    public interface IRequirement
    {
        public bool Passes(Object obj);
        public delegate bool Check(Object obj);
    }

    public class CharacteristicRequirement : IRequirement
    {
        private readonly Enum Characteristic;
        private readonly bool RequiredValue;

        private CharacteristicRequirement(Enum characteristic, bool requiredValue = true)
        {
            Characteristic = characteristic;
            RequiredValue = requiredValue;
        }
        
        public bool Passes(Object obj)
        {
            return obj.Characteristics.Has(Characteristic) == RequiredValue;
        }

        static public IRequirement.Check Create(Enum characteristic, bool requiredValue = true)
        {
            var req = new CharacteristicRequirement(characteristic, requiredValue);
            return obj => req.Passes(obj);
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

        public bool Passes(Object obj)
        {
            return Evaluation(obj.Attributes.GetAttributeValue(Attribute), Threshold);
        }
        
        static public IRequirement.Check Create(Enum attribute, int threshold, Evaluate evaluation)
        {
            var req = new AttributeRequirement(attribute, threshold, evaluation);
            return obj => req.Passes(obj);
        }
    }

    public class StateRequirement<S> : IRequirement where S: State, new()
    {
        private readonly Enum Trigger;
        private readonly bool Permitted;

        private StateRequirement(Enum trigger, bool permitted = true)
        {
            Trigger = trigger;
            Permitted = permitted;
        }

        public bool Passes(Object obj)
        {
            return obj.IsPermittedTrigger<S>(Trigger) == Permitted;
        }
        
        static public IRequirement.Check Create(Enum trigger, bool permitted = true)
        {
            var req = new StateRequirement<S>(trigger, permitted);
            return obj => req.Passes(obj);
        }
    }
}