using System;
using AldursLab.WurmAssistant3.Areas.Persistence;
using AldursLab.WurmAssistant3.Areas.Timers.Custom;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.Timers
{
    [KernelBind(BindingHint.Singleton)]
    public class TimerInstances
    {
        readonly IPersistentObjectResolver persistentObjectResolver;
        readonly TimerTypes timerTypes;
        readonly TimerDefinitions timerDefinitions;

        public TimerInstances([NotNull] IPersistentObjectResolver persistentObjectResolver,
            [NotNull] TimerTypes timerTypes, [NotNull] TimerDefinitions timerDefinitions)
        {
            if (persistentObjectResolver == null) throw new ArgumentNullException("persistentObjectResolver");
            if (timerTypes == null) throw new ArgumentNullException("timerTypes");
            if (timerDefinitions == null) throw new ArgumentNullException("timerDefinitions");
            this.persistentObjectResolver = persistentObjectResolver;
            this.timerTypes = timerTypes;
            this.timerDefinitions = timerDefinitions;
        }

        public WurmTimer CreateTimer(Guid definitionId)
        {
            return BuildTimer(definitionId, Guid.NewGuid());
        }

        public WurmTimer GetTimer(Guid definitionId, Guid timerId)
        {
            var persistentId = timerId;
            var definition = timerDefinitions.GetById(definitionId);

            object newTimer = persistentObjectResolver.Get(persistentId.ToString(),
                timerTypes.GetTypeForId(definition.RuntimeTypeId));

            var timer = newTimer as CustomTimer;
            if (timer != null)
            {
                timer.ApplyCustomTimerOptions(definition.CustomTimerConfig);
            }
            return (WurmTimer)newTimer;
        }

        WurmTimer BuildTimer(Guid definitionId, Guid timerId)
        {
            var persistentId = timerId.ToString();
            var definition = timerDefinitions.GetById(definitionId);

            object newTimer = persistentObjectResolver.Get(persistentId,
                timerTypes.GetTypeForId(definition.RuntimeTypeId));

            var timer = newTimer as CustomTimer;
            if (timer != null)
            {
                timer.ApplyCustomTimerOptions(definition.CustomTimerConfig);
            }
            return (WurmTimer)newTimer;
        }

        public void UnloadAndDeleteTimer([NotNull] WurmTimer timer)
        {
            if (timer == null)
                throw new ArgumentNullException("timer");
            persistentObjectResolver.UnloadAndDeleteData(timer);
        }
    }
}