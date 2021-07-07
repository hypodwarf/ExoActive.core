﻿using System;
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

    public class CapabilityStateProcess<TStateMachine> : ICapabilityProcess
        where TStateMachine : EntityStateMachine, new()
    {
        public static Action<CapabilityProcessData> FireAction(Enum trigger)
        {
            // return data => data.subject.GetState<TStateMachine>().Fire(trigger, data);
            return data => data.actors.ForEach(actor => actor.GetState<TStateMachine>().Fire(trigger, data));
        }

        // public static CapabilityStateProcess<S> CreateFireAction(Enum trigger)
        // {
        //     return Create(
        //         new[]
        //         {
        //             FireAction(trigger)
        //         },
        //         new[]
        //         {
        //             StateRequirement<S>.Create(trigger)
        //         });
        // }

        public static CapabilityStateProcess<TStateMachine> Create(
            Action<CapabilityProcessData>[] actions, IRequirement.Check[] requirements)
        {
            var capabilityProcess = new CapabilityStateProcess<TStateMachine>();
            foreach (var action in actions) capabilityProcess.ActionEvent += action;

            foreach (var requirement in requirements) capabilityProcess.Requirements.Add(requirement);

            return capabilityProcess;
        }

        protected event Action<CapabilityProcessData> ActionEvent;

        protected readonly IList<IRequirement.Check> Requirements = new List<IRequirement.Check>();

        protected static TStateMachine GetState(IEntity entity)
        {
            return entity.GetState<TStateMachine>();
        }

        protected virtual TStateMachine CreateState()
        {
            return StateHelper<TStateMachine>.CreateState();
        }

        public bool PassesRequirements(CapabilityProcessData data)
        {
            // if (!data.subject.HasState<TStateMachine>()) data.subject.AddState(CreateState());
            // return Requirements.All(req => req(data.subject, data));
            
            // Make sure all actors have the state
            data.actors.ForEach(actor =>
            {
                if (!actor.HasState<TStateMachine>()) actor.AddState(CreateState());
            });
            return Requirements.All(req => req(data));
        }

        public virtual void PerformAction(CapabilityProcessData data)
        {
            ActionEvent?.Invoke(data);
        }
    }

    /**
         * The CapabilityTriggerProcess runs as part of a Capaability. It is explicitly associated with a State and a Trigger.
         * The State is added to the subject IEntity if it not already available.
         * The trigger requirement is added by default.
         */
    public class CapabilityTriggerProcess<TStateMachine> : ICapabilityProcess
        where TStateMachine : EntityStateMachine, new()
    {
        private static readonly Dictionary<Enum, CapabilityTriggerProcess<TStateMachine>> processes =
            new();

        public static CapabilityTriggerProcess<TStateMachine> Get(Enum trigger)
        {
            if (!processes.TryGetValue(trigger, out var process))
            {
                process = new CapabilityTriggerProcess<TStateMachine>(trigger);
                processes[trigger] = process;
            }

            return process;
        }

        private readonly Enum trigger;
        private readonly IRequirement.Check requirement;

        private CapabilityTriggerProcess(Enum trigger)
        {
            this.trigger = trigger;
            this.requirement = StateRequirement<TStateMachine>.Create(trigger);
        }

        public bool PassesRequirements(CapabilityProcessData data)
        {
            // Make sure all actors have the state
            data.actors.ForEach(actor =>
            {
                if (!actor.HasState<TStateMachine>()) actor.AddState(StateHelper<TStateMachine>.CreateState());
            });
            return requirement(data);
        }

        public void PerformAction(CapabilityProcessData data)
        {
            // data.subject.GetState<TStateMachine>().Fire(trigger, data);
            data.actors.ForEach(actor => actor.GetState<TStateMachine>().Fire(trigger, data));
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


        private readonly List<ICapabilityProcess> actorActions = new();
        private readonly List<ICapabilityProcess> targetActions = new();

        protected Capability(ICapabilityProcess[] actorActions,
            ICapabilityProcess[] targetActions = null)
        {
            this.actorActions.AddRange(actorActions);
            this.targetActions.AddRange(targetActions ?? Array.Empty<ICapabilityProcess>());
        }

        public bool PassesRequirements(List<IEntity> actors, List<IEntity> targets = null)
        {
            return actorActions.All(action => action.PassesRequirements(
                           new CapabilityProcessData(actors, targets))) &&
                   targetActions.All(action => action.PassesRequirements(
                           new CapabilityProcessData(targets, actors)));
        }

        public bool PerformAction(List<IEntity> actors, List<IEntity> targets = null)
        {
            if (PassesRequirements(actors, targets))
            {
                BeforeAction(actors, targets);
                actorActions.ForEach(action => action.PerformAction(new CapabilityProcessData(actors, targets)));
                targetActions.ForEach(action => action.PerformAction(new CapabilityProcessData(targets, actors)));
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