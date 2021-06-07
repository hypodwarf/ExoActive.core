using System.Collections.Generic;
using System.Linq;

namespace ExoActive
{
    public abstract class Capability
    {
        // private Object owner;
        protected readonly IList<IRequirement.Check> OwnerRequirements = new List<IRequirement.Check>();
        protected readonly IList<IRequirement.Check> TargetRequirements = new List<IRequirement.Check>();

        public Capability(bool requiresTarget, bool canTargetSelf)
        {
            RequiresTarget = requiresTarget;
            CanTargetSelf = canTargetSelf;
        }

        public bool RequiresTarget { get; }
        public bool CanTargetSelf{ get; }

        Object ResolveTarget(Object owner, Object target = null)
        {
            if (RequiresTarget)
            {
                if (target != null)
                {
                    return target;
                }
                
                if (CanTargetSelf)
                {
                    return owner;
                }
            }
            return null;
        }
        
        public bool CanPerform(Object owner, Object target = null)
        {
            return _canPerform(owner, ResolveTarget(owner, target));
        }

        private bool _canPerform(Object owner, Object resolvedTarget)
        {
            bool ownerCanPerform = OwnerRequirements.All(req => req(owner));
            bool targetCanPerform = resolvedTarget != null ? TargetRequirements.All(req => req(resolvedTarget)) : !RequiresTarget;
            return ownerCanPerform && targetCanPerform;
        }

        protected abstract bool Action(Object owner, Object target = null);

        public bool Perform(Object owner, Object target = null)
        {
            Object resolvedTarget = ResolveTarget(owner, target);
            return _canPerform(owner, resolvedTarget) && Action(owner, resolvedTarget);
        }
    }
}