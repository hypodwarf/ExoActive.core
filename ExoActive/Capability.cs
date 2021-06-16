using System;
using System.Collections.Generic;
using System.Linq;

namespace ExoActive
{
    public interface ICapability
    {
        public bool PassesRequirements(List<Entity> actors, List<Entity> targets);
        public bool PerformAction(List<Entity> actors, List<Entity> targets);
    }

    public interface ICapabilityAction
    {
        public bool PassesRequirements(Entity subject);
        public void PerformAction(Entity subject);
    }

    public class CapabilityAction<S> : ICapabilityAction where S : State, new()
    {
        public static Action<Entity> FireAction(Enum trigger)
        {
            return entity => entity.GetState<S>().Fire(trigger);
        }

        public static CapabilityAction<S> CreateFireAction(Enum trigger)
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

        public static CapabilityAction<S> Create(Action<Entity>[] actions, IRequirement.Check[] requirements)
        {
            var capabilityAction = new CapabilityAction<S>();
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

        public bool PassesRequirements(Entity entity)
        {
            if (!entity.HasState<S>()) entity.AddState(CreateState());
            return Requirements.All(req => req(entity));
        }

        public virtual void PerformAction(Entity entity)
        {
            ActionEvent?.Invoke(entity);
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


        private readonly List<ICapabilityAction> actorActions = new();
        private readonly List<ICapabilityAction> targetActions = new();

        protected Capability(ICapabilityAction[] actorActions, ICapabilityAction[] targetActions = null)
        {
            this.actorActions.AddRange(actorActions);
            this.targetActions.AddRange(targetActions ?? Array.Empty<ICapabilityAction>());
        }

        public bool PassesRequirements(List<Entity> actors, List<Entity> targets = null)
        {
            return actorActions.All(action => actors.All(action.PassesRequirements)) &&
                   targetActions.All(action => (targets ?? new List<Entity>()).All(action.PassesRequirements));
        }

        public bool PerformAction(List<Entity> actors, List<Entity> targets = null)
        {
            if (PassesRequirements(actors, targets))
            {
                actorActions.ForEach(action => actors.ForEach(action.PerformAction));
                if (targets != null) targetActions.ForEach(action => targets.ForEach(action.PerformAction));
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