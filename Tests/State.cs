using System;
using System.Linq;
using ExoActive;
using NUnit.Framework;

namespace Tests
{
    public class Cup : State
    {
        public enum State
        {
            Empty,
            HalfFull,
            Full
        }

        public enum Trigger
        {
            Fill,
            Drink
        }
        
        public override Array States { get => Enum.GetValues(typeof(State)); }
        public override Array Triggers { get => Enum.GetValues(typeof(Trigger)); }

        public int Amount { get; private set; }

        public Cup(State? initialState = null) : base("StateContainer", initialState ?? State.Empty)
        {
            machine.Configure(State.Empty)
                .OnEntry(() => Amount = 0)
                .Permit(Trigger.Fill, State.HalfFull);
            
            machine.Configure(State.HalfFull)
                .OnEntryFrom(Trigger.Fill, () => Amount = 50)
                .OnEntryFrom(Trigger.Drink, () => Amount = 50)
                .Permit(Trigger.Fill, State.Full)
                .Permit(Trigger.Drink, State.Empty);
            
            machine.Configure(State.Full)
                .OnEntry(() => Amount = 100)
                .Permit(Trigger.Drink, State.HalfFull);
        }
    }
    
    public class DynamicCup : State
    {
        public enum State
        {
            Empty,
            HalfFull,
            Full
        }

        public enum Trigger
        {
            Fill,
            Drink
        }
        
        public override Array States { get => Enum.GetValues(typeof(State)); }
        public override Array Triggers { get => Enum.GetValues(typeof(Trigger)); }
        
        public static readonly TriggerWithParameters<int> FillSome = new TriggerWithParameters<int>(Trigger.Fill);
        public static readonly TriggerWithParameters<string> FillSomeS = new TriggerWithParameters<string>(Trigger.Fill);
        public static readonly TriggerWithParameters<int> DrinkSome = new TriggerWithParameters<int>(Trigger.Drink);

        public int Amount { get; private set; }
        
        private State nextState(int change)
        {
            Amount += change;
            return Amount >= 100 ? State.Full : Amount <= 0 ? State.Empty : State.HalfFull;
        }

        private State nextState(string change)
        {
            Console.WriteLine(change);
            return State.Empty;
        }

        public DynamicCup(State? initialState = null) : base("StateContainer", initialState ?? State.Empty)
        {
            machine.Configure(State.Empty)
                .PermitDynamic<int>(FillSome, i => nextState(i));
            
            machine.Configure(State.HalfFull)
                .PermitDynamic(FillSome, i => nextState(i))
                .PermitDynamic(DrinkSome, i => nextState(-i));
            
            machine.Configure(State.Full)
                .PermitDynamic(DrinkSome, i => nextState(-i));
        }
    }
    
    public class StateTest
    {
        [Test]
        public void FireEmptyTriggers()
        {
            var cup = new Cup();
            Assert.AreEqual(0, cup.Amount);
            Assert.AreEqual(Cup.State.Empty, cup.CurrentState);
            cup.Fire(Cup.Trigger.Fill);
            Assert.AreEqual(50, cup.Amount);
            Assert.AreEqual(Cup.State.HalfFull, cup.CurrentState);
            cup.Fire(Cup.Trigger.Fill);
            Assert.AreEqual(100, cup.Amount);
            Assert.AreEqual(Cup.State.Full, cup.CurrentState);
            cup.Fire(Cup.Trigger.Drink);
            Assert.AreEqual(50, cup.Amount);
            Assert.AreEqual(Cup.State.HalfFull, cup.CurrentState);
            cup.Fire(Cup.Trigger.Drink);
            Assert.AreEqual(0, cup.Amount);
            Assert.AreEqual(Cup.State.Empty, cup.CurrentState);
        }
        
        [Test]
        public void FireDynamicTriggers()
        {
            var cup = new DynamicCup();
            
            Assert.AreEqual(0, cup.Amount);
            Assert.AreEqual(DynamicCup.State.Empty, cup.CurrentState);
            cup.Fire(DynamicCup.FillSome, 50);
            Assert.AreEqual(50, cup.Amount);
            Assert.AreEqual(DynamicCup.State.HalfFull, cup.CurrentState);
            cup.Fire(DynamicCup.FillSome, 50);
            Assert.AreEqual(100, cup.Amount);
            Assert.AreEqual(DynamicCup.State.Full, cup.CurrentState);
            cup.Fire(DynamicCup.DrinkSome, 50);
            Assert.AreEqual(50, cup.Amount);
            Assert.AreEqual(DynamicCup.State.HalfFull, cup.CurrentState);
            cup.Fire(DynamicCup.DrinkSome, 50);
            Assert.AreEqual(0, cup.Amount);
            Assert.AreEqual(DynamicCup.State.Empty, cup.CurrentState);
            
        }
    }
}