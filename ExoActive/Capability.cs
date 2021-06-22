using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;

namespace ExoActive
{
    public struct CapabilityProcessData
    {
        [Flags] public enum DataFilter
        {
            Subject = 1 << 0,
            Actors = 1 << 1,
            Targets = 1 << 2
        }
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
        public static Action<CapabilityProcessData> FireAction(Enum trigger)
        {
            return data => data.subject.GetState<S>().Fire(trigger, data);
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

        public static CapabilityStateProcess<S> Create(Action<CapabilityProcessData>[] actions, IRequirement.Check[] requirements)
        {
            var capabilityProcess = new CapabilityStateProcess<S>();
            foreach (var action in actions) capabilityProcess.ActionEvent += action;

            foreach (var requirement in requirements) capabilityProcess.Requirements.Add(requirement);

            return capabilityProcess;
        }

        protected event Action<CapabilityProcessData> ActionEvent;
        
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
            ActionEvent?.Invoke(data);
        }
    }

    /**
     * The CapabilityTriggerProcess runs as part of a Capaability. It is explicitly associated with a State and a Trigger.
     * The State is added to the subject Entity if it not already available.
     * The trigger requirement is added by default.
     */
    public class CapabilityTriggerProcess<S> : ICapabilityProcess where S : State, new()
    {
        private static readonly Dictionary<(Enum, CapabilityProcessData.DataFilter), CapabilityTriggerProcess<S>> processes = new ();

        public static CapabilityTriggerProcess<S> Get(Enum trigger, CapabilityProcessData.DataFilter dataFilter = CapabilityProcessData.DataFilter.Subject)
        {
            if (!processes.TryGetValue((trigger, dataFilter), out var process))
            {
                process = new CapabilityTriggerProcess<S>(trigger, dataFilter);
                processes[(trigger, dataFilter)] = process;
            }

            return process;
        }

        private readonly Enum trigger;
        private readonly CapabilityProcessData.DataFilter dataFilter;
        private readonly IRequirement.Check requirement;
        
        private CapabilityTriggerProcess(Enum trigger, CapabilityProcessData.DataFilter dataFilter)
        {
            this.trigger = trigger;
            this.dataFilter = dataFilter;
            this.requirement = StateRequirement<S>.Create(trigger);
        }

        private bool PassesRequirements(CapabilityProcessData data, Entity entity)
        {
            if (!entity.HasState<S>()) entity.AddState(StateHelper<S>.CreateState());
            return requirement(entity);
        }

        public bool PassesRequirements(CapabilityProcessData data)
        {
            bool passes = true;
            if (dataFilter.HasFlag(CapabilityProcessData.DataFilter.Subject))
            {
                passes &= PassesRequirements(data, data.subject);
            }
            
            if (passes && dataFilter.HasFlag(CapabilityProcessData.DataFilter.Actors))
            {
                passes &= data.actors.All(actor => PassesRequirements(data, actor));
            }
            
            if (passes && dataFilter.HasFlag(CapabilityProcessData.DataFilter.Targets))
            {
                passes &= data.targets.All(target => PassesRequirements(data, target));
            }

            return passes;
        }

        private void PerformAction(CapabilityProcessData data, Entity entity)
        {
            entity.GetState<S>().Fire(trigger, data);
        }

        public void PerformAction(CapabilityProcessData data)
        {
            if (dataFilter.HasFlag(CapabilityProcessData.DataFilter.Subject))
            {
                PerformAction(data, data.subject);
            }
            
            if (dataFilter.HasFlag(CapabilityProcessData.DataFilter.Actors))
            {
                data.actors.ForEach(actor => PerformAction(data, actor));
            }
            
            if (dataFilter.HasFlag(CapabilityProcessData.DataFilter.Targets))
            {
                data.targets.ForEach(target => PerformAction(data, target));
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