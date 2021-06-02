using System;

namespace ExoActive
{
    public interface IRequirement
    {
        public bool Passes(Object obj);
    }

    public class Requirement
    {
        private readonly Object obj;
        private readonly IRequirement requirement;

        public Requirement(Object obj, IRequirement requirement)
        {
            this.obj = obj;
            this.requirement = requirement;
        }

        private bool Value
        {
            get => requirement.Passes(obj);
        }

        public static implicit operator bool(Requirement req) => req.Value;
    }
    

    public class CharacteristicRequirement : IRequirement
    {
        private readonly Enum Characteristic;
        private readonly bool RequiredValue;

        public CharacteristicRequirement(Enum characteristic, bool requiredValue = true)
        {
            Characteristic = characteristic;
            RequiredValue = requiredValue;
        }
        
        public bool Passes(Object obj)
        {
            return obj.Characteristics.Has(Characteristic) == RequiredValue;
        }
    }

    public class AttributeRequirement : IRequirement
    {
        public delegate bool Evaluate(int value, int threshold);
        
        private readonly Enum Attribute;
        private readonly int Threshold;
        private readonly Evaluate Evaluation;

        public AttributeRequirement(Enum attribute, int threshold, Evaluate evaluation)
        {
            Attribute = attribute;
            Threshold = threshold;
            Evaluation = evaluation;
        }

        public bool Passes(Object obj)
        {
            return Evaluation(obj.Attributes.GetAttributeValue(Attribute), Threshold);
        }
    }

    public class StateRequirement : IRequirement
    {
        private Enum Trigger;
        private bool Permitted;

        public StateRequirement(Enum trigger, bool permitted = true)
        {
            Trigger = trigger;
            Permitted = permitted;
        }

        public bool Passes(Object obj)
        {
            return obj.PermittedTriggers.Contains(Trigger) == Permitted;
        }
    }
}