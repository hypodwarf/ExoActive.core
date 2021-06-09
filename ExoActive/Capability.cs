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
    
    public abstract class CapabilityAction: ICapabilityAction
    {
        protected readonly IList<IRequirement.Check> Requirements = new List<IRequirement.Check>();
        
        protected abstract String StateId { get; }

        protected abstract State CreateState();

        public bool PassesRequirements(Object obj)
        {
            if (!obj.HasState(StateId))
            {
                obj.AddState(CreateState());
            }
            return Requirements.All(req => req(obj));
        }

        public abstract void PerformAction(Object obj);
    }
    
    public abstract class Capabilty: ICapability
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