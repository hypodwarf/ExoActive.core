using System;
using System.Collections.Generic;
using System.Linq;

namespace ExoActive
{
    public struct CapabilityProcessData
    {
        public Entity subject;
        public List<Entity> actors;
        public List<Entity> targets;

        public CapabilityProcessData(Entity subject, List<Entity> actors, List<Entity> targets)
        {
            this.subject = subject;
            this.actors = actors;
            this.targets = targets;
        }
    }
    
    public interface ICapability
    {
        public bool PassesRequirements(List<Entity> actors, List<Entity> targets);
        public bool PerformAction(List<Entity> actors, List<Entity> targets);
    }

    public interface ICapabilityProcess
    {
        public bool PassesRequirements(CapabilityProcessData data);
        public void PerformAction(CapabilityProcessData data);
    }
    
    public class CapabilityStateProcess<S> : ICapabilityProcess where S : State, new()
    {
        public static Action<Entity> FireAction(Enum trigger)
        {
            return entity => entity.GetState<S>().Fire(trigger);
        }

        public static CapabilityStateProcess<S> CreateFireAction(Enum trigger)
        {
            return Create(
                new[]
                {
                    FireAction(trigger)
                },
                new[]
                {
                    StateRequirement<S>.Create(trigger)
                });
        }

        public static CapabilityStateProcess<S> Create(Action<Entity>[] actions, IRequirement.Check[] requirements)
        {
            var capabilityAction = new CapabilityStateProcess<S>();
            foreach (var action in actions) capabilityAction.ActionEvent += action;

            foreach (var requirement in requirements) capabilityAction.Requirements.Add(requirement);

            return capabilityAction;
        }

        protected event Action<Entity> ActionEvent;
        protected readonly IList<IRequirement.Check> Requirements = new List<IRequirement.Check>();

        protected static State GetState(Entity entity)
        {
            return entity.GetState<S>();
        }

        protected virtual S CreateState()
        {
            return StateHelper<S>.CreateState();
        }

        public bool PassesRequirements(CapabilityProcessData data)
        {
            if (!data.subject.HasState<S>()) data.subject.AddState(CreateState());
            return Requirements.All(req => req(data.subject));
        }

        public virtual void PerformAction(CapabilityProcessData data)
        {
            ActionEvent?.Invoke(data.subject);
        }
    }

    /**
     * The CapabilityTriggerProcess runs as part of a Capaability. It is explicitly associated with a State and a Trigger.
     * The State is added to the subject Entity if it not already available.
     * The trigger requirement is added by default.
     */
    public class CapabilityTriggerProcess<S> : ICapabilityProcess where S : State, new()
    {
        private static readonly Dictionary<Enum, CapabilityTriggerProcess<S>> processes = new ();

        public static CapabilityTriggerProcess<S> Get(Enum trigger)
        {
            if (!processes.TryGetValue(trigger, out var process))
            {
                process = new CapabilityTriggerProcess<S>(trigger);
                processes[trigger] = process;
            }

            return process;
        }

        private readonly Enum trigger;
        private readonly IRequirement.Check requirement;
        private CapabilityTriggerProcess(Enum trigger)
        {
            this.trigger = trigger;
            this.requirement = StateRequirement<S>.Create(trigger);
        }

        public bool PassesRequirements(CapabilityProcessData data)
        {
            if (!data.subject.HasState<S>()) data.subject.AddState(StateHelper<S>.CreateState());
            return requirement(data.subject);
        }

        public virtual void PerformAction(CapabilityProcessData data)
        {
            data.subject.GetState<S>().Fire(trigger, data);
        }
    }

    public abstract class Capability : ICapability
    {
        private static readonly Dictionary<Type, Capability> capabilities = new();

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

        public static bool PerformAction<C>(List<Entity> actors, List<Entity> targets = null)
            where C : Capability, new()
        {
            return Get<C>().PerformAction(actors, targets);
        }

        public static bool PerformAction<C>(Entity actor, params Entity[] targets) where C : Capability, new()
        {
            return PerformAction<C>(new List<Entity> {actor}, targets.ToList());
        }


        private readonly List<ICapabilityProcess> actorActions = new();
        private readonly List<ICapabilityProcess> targetActions = new();

        protected Capability(ICapabilityProcess[] actorActions, ICapabilityProcess[] targetActions = null)
        {
            this.actorActions.AddRange(actorActions);
            this.targetActions.AddRange(targetActions ?? Array.Empty<ICapabilityProcess>());
        }

        public bool PassesRequirements(List<Entity> actors, List<Entity> targets = null)
        {
            return actorActions.All(action => actors.All(actor => action.PassesRequirements(new CapabilityProcessData(actor, actors, targets)))) &&
                   targetActions.All(action => (targets ?? new List<Entity>()).All(target => action.PassesRequirements(new CapabilityProcessData(target, actors, targets))));
        }

        public bool PerformAction(List<Entity> actors, List<Entity> targets = null)
        {
            if (PassesRequirements(actors, targets))
            {
                actorActions.ForEach(action => actors.ForEach(actor => action.PerformAction(new CapabilityProcessData(actor, actors, targets))));
                if (targets != null) targetActions.ForEach(action => targets.ForEach(target => action.PerformAction(new CapabilityProcessData(target, actors, targets))));
                return true;
            }

            return false;
        }

        public bool PerformAction(Entity actor, params Entity[] targets)
        {
            return PerformAction(new List<Entity> {actor}, targets.ToList());
        }
    }
}