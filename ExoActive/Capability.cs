using System;
using System.Collections.Generic;
using System.Linq;

namespace ExoActive
{
    
    public interface ICapability
    {
        public bool PassesRequirements(List<Object> actors, List<Object> targets);
        public bool PerformAction(List<Object> actors, List<Object> targets);
    }
    
    public interface ICapabilityAction
    {
        public bool PassesRequirements(Object subject);
        public void PerformAction(Object subject);
    }
    
    public class CapabilityAction<S> : ICapabilityAction where S : State, new()
    {
        public static Action<Object> FireAction(Enum trigger)
        {
            return obj => obj.GetState<S>().Fire(trigger);
        }

        public static CapabilityAction<S> CreateFireAction(Enum trigger)
        {
            return Create(
                new[] {
                    CapabilityAction<S>.FireAction(trigger)
                },
                new[] {
                    StateRequirement<S>.Create(trigger)
                });
        }
        
        public static CapabilityAction<S> Create(Action<Object>[] actions, IRequirement.Check[] requirements)
        {
            var capabilityAction = new CapabilityAction<S>();
            foreach (var action in actions)
            {
                capabilityAction.ActionEvent += action;
            }

            foreach (var requirement in requirements)
            {
                capabilityAction.Requirements.Add(requirement);
            }

            return capabilityAction;
        }

        protected event Action<Object> ActionEvent ;
        protected readonly IList<IRequirement.Check> Requirements = new List<IRequirement.Check>();

        protected static State GetState(Object obj) => obj.GetState<S>();
        protected virtual S CreateState() => StateHelper<S>.CreateState();

        public bool PassesRequirements(Object obj)
        {
            if (!obj.HasState<S>())
            {
                obj.AddState(CreateState());
            }
            return Requirements.All(req => req(obj));
        }

        public virtual void PerformAction(Object obj)
        {
            ActionEvent?.Invoke(obj);
        }
    }
    
    public abstract class Capability: ICapability
    {
        protected readonly List<ICapabilityAction> actorActions = new List<ICapabilityAction>();
        protected readonly List<ICapabilityAction> targetActions = new List<ICapabilityAction>();

        public bool PassesRequirements(List<Object> actors, List<Object> targets = null)
        {
            return actorActions.All(action => actors.All(action.PassesRequirements)) &&
                   targetActions.All(action => (targets ?? new List<Object>()).All(action.PassesRequirements));
        }

        public bool PerformAction(List<Object> actors, List<Object> targets = null)
        {
            if (PassesRequirements(actors, targets))
            {
                actorActions.ForEach(action => actors.ForEach(action.PerformAction));
                if (targets != null)
                {
                    targetActions.ForEach(action => targets.ForEach(action.PerformAction));
                }
                return true;
            }

            return false;
        }
    }
}