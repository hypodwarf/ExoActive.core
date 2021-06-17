using ExoActive;
using NUnit.Framework;

namespace Tests
{
    public class Cup : State
    {
        public new enum State
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

        public int Amount { get; private set; }

        public Cup() : this(State.Empty)
        {
        }

        public Cup(State initialState) : base(initialState)
        {
            Configure(State.Empty)
                .OnEntry(() => Amount = 0)
                .PermitIf(GetTrigger(Trigger.Fill), State.HalfFull);

            Configure(State.HalfFull)
                .OnEntryFrom(Trigger.Fill, () => Amount = 50)
                .OnEntryFrom(Trigger.Drink, () => Amount = 50)
                .PermitIf(GetTrigger(Trigger.Fill), State.Full)
                .PermitIf(GetTrigger(Trigger.Drink), State.Empty);

            Configure(State.Full)
                .OnEntry(() => Amount = 100)
                .PermitIf(GetTrigger(Trigger.Drink), State.HalfFull);
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
            Drink,
            Evaporate
        }

        public static readonly TriggerWithParameters<int> FillSome = new(Trigger.Fill);
        public static readonly TriggerWithParameters<int> DrinkSome = new(Trigger.Drink);

        public int Amount { get; private set; }

        private State nextState(int change)
        {
            Amount += change;
            return Amount >= 100 ? State.Full : Amount <= 0 ? State.Empty : State.HalfFull;
        }

        protected override void OnTickEvent()
        {
            if (CanFire(Trigger.Evaporate)) Fire(Trigger.Evaporate);
        }

        public DynamicCup() : this(State.Empty)
        {
        }

        public DynamicCup(State initialState) : base(initialState)
        {
            Configure(State.Empty)
                .PermitDynamic(FillSome, i => nextState(i));

            Configure(State.HalfFull)
                .PermitDynamic(FillSome, i => nextState(i))
                .PermitDynamic(DrinkSome, i => nextState(-i))
                .PermitDynamicIf(Trigger.Evaporate, () => nextState(
                    -(int) (TimeTicker.Ticks - LastTransitionTick)), () => TimeTicker.Ticks > LastTransitionTick);

            Configure(State.Full)
                .PermitDynamic(DrinkSome, i => nextState(-i))
                .PermitDynamicIf(Trigger.Evaporate, () => nextState(
                    -(int) (TimeTicker.Ticks - LastTransitionTick)), () => TimeTicker.Ticks > LastTransitionTick);
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

        [Test]
        public void StateTransitionEvent()
        {
            var cup = new DynamicCup();
            var transCompleteCount = 0;
            cup.OnTransitionCompleted += _ => transCompleteCount++;

            cup.Fire(DynamicCup.FillSome, 25);
            Assert.AreEqual(1, transCompleteCount);
            cup.Fire(DynamicCup.FillSome, 25);
            Assert.AreEqual(2, transCompleteCount);
            cup.Fire(DynamicCup.DrinkSome, 25);
            Assert.AreEqual(3, transCompleteCount);
            cup.Fire(DynamicCup.DrinkSome, 25);
            Assert.AreEqual(4, transCompleteCount);
        }

        [Test]
        public void StateTime()
        {
            var cup = new DynamicCup();

            Assert.AreEqual(0, TimeTicker.Ticks);
            Assert.AreEqual(0, cup.LastTransitionTick);

            TimeTicker.AddTicks(100);
            Assert.AreEqual(100, TimeTicker.Ticks);
            Assert.AreEqual(0, cup.LastTransitionTick);

            TimeTicker.AddTicks(100);

            Assert.AreEqual(200, TimeTicker.Ticks);
            Assert.AreEqual(0, cup.LastTransitionTick);

            cup.Fire(DynamicCup.FillSome, 25);
            Assert.AreEqual(25, cup.Amount);

            Assert.AreEqual(200, TimeTicker.Ticks);
            Assert.AreEqual(200, cup.LastTransitionTick);

            TimeTicker.AddTicks(1);

            Assert.AreEqual(24, cup.Amount);
        }
    }
}