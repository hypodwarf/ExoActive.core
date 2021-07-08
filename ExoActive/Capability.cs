using System;
using System.Collections.Generic;
using System.Linq;

namespace ExoActive
{
    public readonly struct CapabilityProcessData
    {
        // public readonly IEntity subject;
        public readonly List<IEntity> actors;
        public readonly List<IEntity> targets;

        public CapabilityProcessData(List<IEntity> actors,
            List<IEntity> targets)
        {
            // this.subject = subject;
            this.actors = actors;
            this.targets = targets;
        }
    }

    [Flags]
    public enum DataSelect : byte
    {
        Actors = 1 << 0,
        Targets = 1 << 1,
        Both = Actors | Targets
    }

    public interface ICapability
    {
        public bool PassesRequirements(List<IEntity> actors, List<IEntity> targets);
        public bool PerformAction(List<IEntity> actors, List<IEntity> targets);
    }

    public interface ICapabilityProcess
    {
        public bool PassesRequirements(CapabilityProcessData data);
        public void PerformAction(CapabilityProcessData data);
    }

    // public class CapabilityStateProcess<TStateMachine> : ICapabilityProcess
    //     where TStateMachine : EntityStateMachine, new()
    // {
    //     public static Action<CapabilityProcessData> FireAction(Enum trigger)
    //     {
    //         return data => data.actors.ForEach(actor => actor.GetState<TStateMachine>().Fire(trigger, data));
    //     }
    //
    //     public static CapabilityStateProcess<TStateMachine> Create(
    //         Action<CapabilityProcessData>[] actions, IRequirement.Check[] requirements)
    //     {
    //         var capabilityProcess = new CapabilityStateProcess<TStateMachine>();
    //         foreach (var action in actions) capabilityProcess.ActionEvent += action;
    //
    //         foreach (var requirement in requirements) capabilityProcess.Requirements.Add(requirement);
    //
    //         return capabilityProcess;
    //     }
    //
    //     protected event Action<CapabilityProcessData> ActionEvent;
    //
    //     protected readonly IList<IRequirement.Check> Requirements = new List<IRequirement.Check>();
    //
    //     protected static TStateMachine GetState(IEntity entity)
    //     {
    //         return entity.GetState<TStateMachine>();
    //     }
    //
    //     protected virtual TStateMachine CreateState()
    //     {
    //         return StateHelper<TStateMachine>.CreateState();
    //     }
    //
    //     public bool PassesRequirements(CapabilityProcessData data)
    //     {
    //         // Make sure all actors have the state
    //         data.actors.ForEach(actor =>
    //         {
    //             if (!actor.HasState<TStateMachine>()) actor.AddState(CreateState());
    //         });
    //         return Requirements.All(req => req(data));
    //     }
    //
    //     public virtual void PerformAction(CapabilityProcessData data)
    //     {
    //         ActionEvent?.Invoke(data);
    //     }
    // }

    /**
         * The CapabilityTriggerProcess runs as part of a Capaability. It is explicitly associated with a State and a Trigger.
         * The State is added to the subject IEntity if it not already available.
         * The trigger requirement is added by default.
         */
    public class CapabilityTriggerProcess<TStateMachine> : ICapabilityProcess
        where TStateMachine : EntityStateMachine, new()
    {
        private static readonly Dictionary<(Enum, DataSelect), CapabilityTriggerProcess<TStateMachine>> processes =
            new();

        public static CapabilityTriggerProcess<TStateMachine> Get(Enum trigger, DataSelect selector)
        {
            if (!processes.TryGetValue((trigger, selector), out var process))
            {
                process = new CapabilityTriggerProcess<TStateMachine>(trigger, selector);
                processes[(trigger, selector)] = process;
            }

            return process;
        }

        private readonly Enum trigger;
        private readonly DataSelect selector;
        private readonly IRequirement.Check requirement;

        private CapabilityTriggerProcess(Enum trigger, DataSelect selector)
        {
            this.trigger = trigger;
            this.selector = selector;
            this.requirement = StateRequirement<TStateMachine>.Create(trigger, selector);
        }

        public bool PassesRequirements(CapabilityProcessData data)
        {
            
            if (selector.HasFlag(DataSelect.Actors))
            {
                // Make sure all actors have the state
                data.actors.ForEach(actor =>
                {
                    if (!actor.HasState<TStateMachine>()) actor.AddState(StateHelper<TStateMachine>.CreateState());
                });
            }
            
            if (selector.HasFlag(DataSelect.Targets))
            {
                // Make sure all targets have the state
                data.targets.ForEach(target =>
                {
                    if (!target.HasState<TStateMachine>()) target.AddState(StateHelper<TStateMachine>.CreateState());
                });
            }

            ;
            return requirement(data);
        }

        public void PerformAction(CapabilityProcessData data)
        {
            if (selector.HasFlag(DataSelect.Actors))
            {
                data.actors.ForEach(actor => actor.GetState<TStateMachine>().Fire(trigger, data));
            }
            
            if (selector.HasFlag(DataSelect.Targets))
            {
                data.targets.ForEach(target => target.GetState<TStateMachine>().Fire(trigger, data));
            }
        }
    }

    public class DelegateCheckProcess : ICapabilityProcess
    {
        public static DelegateCheckProcess IsTrue(DelegateCheck check)
        {
            return new DelegateCheckProcess(check, true);
        }

        public static DelegateCheckProcess IsFalse(DelegateCheck check)
        {
            return new DelegateCheckProcess(check, false);
        }

        public delegate bool DelegateCheck(CapabilityProcessData data);

        private readonly DelegateCheck check;
        private readonly bool checkResult;

        private DelegateCheckProcess(DelegateCheck check, bool checkResult)
        {
            this.check = check;
            this.checkResult = checkResult;
        }

        public bool PassesRequirements(CapabilityProcessData data)
        {
            return check(data) == checkResult;
        }

        public void PerformAction(CapabilityProcessData data)
        {
        }
    }

    public abstract class Capability : ICapability
    {
        private static readonly Dictionary<Type, Capability> capabilities = new();

        static Capability()
        {
        }

        public static Capability Get<C>() where C : Capability, new()
        {
            try
            {
                return capabilities[typeof(C)];
            }
            catch (KeyNotFoundException)
            {
                var c = new C();
                capabilities.Add(typeof(C), c);

                return c;
            }
        }

        public static bool PerformAction<TCapability>(List<IEntity> actors,
            List<IEntity> targets = null)
            where TCapability : Capability, new()
        {
            return Get<TCapability>().PerformAction(actors, targets);
        }

        public static bool PerformAction<TCapability>(IEntity actor, params IEntity[] targets)
            where TCapability : Capability, new()
        {
            return PerformAction<TCapability>(new List<IEntity> {actor}, targets.ToList());
        }

        public static bool PerformAction<C>(IEntity[] actors, params IEntity[] targets)
            where C : Capability, new()
        {
            return PerformAction<C>(actors.ToList(), targets.ToList());
        }

        private readonly List<ICapabilityProcess> Actions = new();

        protected Capability(params ICapabilityProcess[] Actions)
        {
            this.Actions.AddRange(Actions);
        }

        public bool PassesRequirements(List<IEntity> actors, List<IEntity> targets = null)
        {
            return Actions.All(action => action.PassesRequirements(new CapabilityProcessData(actors, targets)));
        }

        public bool PerformAction(List<IEntity> actors, List<IEntity> targets = null)
        {
            if (PassesRequirements(actors, targets))
            {
                BeforeAction(actors, targets);
                Actions.ForEach(action => action.PerformAction(new CapabilityProcessData(actors, targets)));
                AfterAction(actors, targets);
                return true;
            }

            return false;
        }

        public bool PerformAction(IEntity actor, params IEntity[] targets)
        {
            return PerformAction(new List<IEntity> {actor}, targets.ToList());
        }

        protected virtual void BeforeAction(List<IEntity> actors, List<IEntity> targets)
        {
        }

        protected virtual void AfterAction(List<IEntity> actors, List<IEntity> targets)
        {
        }
    }
}