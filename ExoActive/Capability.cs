﻿using System;
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
        private static readonly Dictionary<Type, Capability> capabilities = new Dictionary<Type, Capability>();

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
        
        public static bool PerformAction<C>(List<Object> actors, List<Object> targets = null) where C : Capability, new () => 
            Get<C>().PerformAction(actors, targets);

        public static bool PerformAction<C>(Object actor, params Object[] targets) where C : Capability, new() =>
            PerformAction<C>(new List<Object> {actor}, targets.ToList());
        

        private readonly List<ICapabilityAction> actorActions = new List<ICapabilityAction>();
        private readonly List<ICapabilityAction> targetActions = new List<ICapabilityAction>();

        protected Capability(ICapabilityAction[] actorActions, ICapabilityAction[] targetActions = null)
        {
            this.actorActions.AddRange(actorActions);
            this.targetActions.AddRange(targetActions ?? Array.Empty<ICapabilityAction>());
        }

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

        public bool PerformAction(Object actor, params Object[] targets) =>  
            PerformAction(new List<Object> {actor}, targets.ToList());
    }
}