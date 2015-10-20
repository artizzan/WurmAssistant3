using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AldursLab.PersistentObjects;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Core.Areas.Persistence.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.Timers.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.Timers.Modules.Timers;
using AldursLab.WurmAssistant3.Core.Areas.Timers.Modules.Timers.Alignment;
using AldursLab.WurmAssistant3.Core.Areas.Timers.Modules.Timers.Custom;
using AldursLab.WurmAssistant3.Core.Areas.Timers.Modules.Timers.JunkSale;
using AldursLab.WurmAssistant3.Core.Areas.Timers.Modules.Timers.Meditation;
using AldursLab.WurmAssistant3.Core.Areas.Timers.Modules.Timers.MeditPath;
using AldursLab.WurmAssistant3.Core.Areas.Timers.Modules.Timers.Prayer;
using AldursLab.WurmAssistant3.Core.Areas.Timers.Modules.Timers.Sermon;
using AldursLab.WurmAssistant3.Core.IoC;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Ninject;

namespace AldursLab.WurmAssistant3.Core.Areas.Timers.Modules
{
    [PersistentObject("TimersFeature_WurmTimerDescriptors")]
    public class TimerDefinitions : PersistentObjectBase, IInitializable
    {
        // default timer kinds are hardcoded
        // custom timers are functionally one kind but are configured by users

        readonly IPersistentObjectResolver persistentObjectResolver;
        readonly TimerTypes timerTypes;
        readonly IWurmApi wurmApi;

        readonly HashSet<TimerDefinition> defaultDefinitions = new HashSet<TimerDefinition>();

        [JsonProperty]
        readonly Dictionary<string, CustomTimerOptionsTemplate> customTimerOptionsTemplates =
            new Dictionary<string, CustomTimerOptionsTemplate>();
        
        [JsonProperty] 
        readonly HashSet<TimerDefinition> customDefinitions = new HashSet<TimerDefinition>();

        readonly HashSet<TimerDefinition> allDefinitions = new HashSet<TimerDefinition>();

        public event EventHandler<CustomTimerRemovedEventArgs> CustomTimerRemoved;

        public TimerDefinitions([NotNull] IPersistentObjectResolver persistentObjectResolver,
            [NotNull] TimerTypes timerTypes, [NotNull] IWurmApi wurmApi)
        {
            if (persistentObjectResolver == null) throw new ArgumentNullException("persistentObjectResolver");
            if (timerTypes == null) throw new ArgumentNullException("timerTypes");
            if (wurmApi == null) throw new ArgumentNullException("wurmApi");
            this.persistentObjectResolver = persistentObjectResolver;
            this.timerTypes = timerTypes;
            this.wurmApi = wurmApi;

            BuildDefaultDescriptor("Meditation", RuntimeTypeId.Meditation);
            BuildDefaultDescriptor("Path Question", RuntimeTypeId.MeditPath);
            BuildDefaultDescriptor("Prayer", RuntimeTypeId.Prayer);
            BuildDefaultDescriptor("Sermon", RuntimeTypeId.Sermon);
            BuildDefaultDescriptor("Alignment", RuntimeTypeId.Alignment);
            BuildDefaultDescriptor("Junk Sale", RuntimeTypeId.JunkSale);
        }

        public void Initialize()
        {
            foreach (var custom in customDefinitions)
            {
                allDefinitions.Add(custom);
            }
        }

        void BuildDefaultDescriptor([NotNull] string nameId, RuntimeTypeId typeId)
        {
            if (nameId == null) throw new ArgumentNullException("nameId");
            foreach (var serverGroup in wurmApi.ServerGroups.All)
            {
                TimerDefinition timertype = new TimerDefinition(new TimerDefinitionId(nameId, serverGroup.ServerGroupId), typeId);
                defaultDefinitions.Add(timertype);
                allDefinitions.Add(timertype);
            }
        }

        public void AddCustomTimerDefinition([NotNull] string nameId,
            [NotNull] CustomTimerOptionsTemplate optionsTemplate)
        {
            if (nameId == null) throw new ArgumentNullException("nameId");
            if (optionsTemplate == null) throw new ArgumentNullException("optionsTemplate");
            foreach (var serverGroup in wurmApi.ServerGroups.All)
            {
                TimerDefinition timertype = new TimerDefinition(
                    new TimerDefinitionId(nameId, serverGroup.ServerGroupId),
                    RuntimeTypeId.LegacyCustom);
                customDefinitions.Add(timertype);
                allDefinitions.Add(timertype);
                customTimerOptionsTemplates[nameId] = optionsTemplate;
                FlagAsChanged();
            }
        }

        public void RemoveCustomTimerDefinition([NotNull] string nameId)
        {
            if (nameId == null) throw new ArgumentNullException("nameId");

            allDefinitions.RemoveWhere(x => x.TimerDefinitionId.Name == nameId);
            customDefinitions.RemoveWhere(x => x.TimerDefinitionId.Name == nameId);
            FlagAsChanged();

            try
            {
                OnCustomTimerRemoved(new CustomTimerRemovedEventArgs(nameId));
            }
            finally
            {
                customTimerOptionsTemplates.Remove(nameId);
            }
        }

        public CustomTimerOptionsTemplate GetOptionsTemplateForCustomTimer([NotNull] string nameId)
        {
            if (nameId == null) throw new ArgumentNullException("nameId");
            return customTimerOptionsTemplates[nameId];
        }

        public TimerDefinition FindDefinitionForTimer([NotNull] WurmTimer timer)
        {
            if (timer == null) throw new ArgumentNullException("timer");
            return allDefinitions.First(x => x.TimerDefinitionId == timer.TimerDefinitionId);
        }

        public HashSet<TimerDefinition> GetDefinitionsOfUnusedTimers(
            [NotNull] HashSet<TimerDefinition> currentActiveTimers)
        {
            if (currentActiveTimers == null) throw new ArgumentNullException("currentActiveTimers");
            HashSet<TimerDefinition> result = new HashSet<TimerDefinition>();
            foreach (var item in allDefinitions)
            {
                if (!currentActiveTimers.Contains(item)) result.Add(item);
            }
            return result;
        }

        public WurmTimer NewTimerFactory([NotNull] TimerDefinition timerDefinition, [NotNull] string characterName)
        {
            if (timerDefinition == null) throw new ArgumentNullException("timerDefinition");
            if (characterName == null) throw new ArgumentNullException("characterName");
            ThrowIfNoDefinition(timerDefinition);

            var persistentId = characterName + "-" + timerDefinition.ToPersistentIdString();

            object newTimer = persistentObjectResolver.Get(persistentId, timerTypes.GetTypeForId(timerDefinition.RuntimeTypeId));

            var timer = newTimer as CustomTimer;
            if (timer != null)
            {
                var latestOptionsTemplate = customTimerOptionsTemplates[timerDefinition.TimerDefinitionId.Name];
                timer.ApplyCustomTimerOptions(latestOptionsTemplate);
            }
            return (WurmTimer)newTimer;
        }

        public bool IsNameUnique([NotNull] string nameId)
        {
            if (nameId == null) throw new ArgumentNullException("nameId");
            return allDefinitions.All(x => x.TimerDefinitionId.Name != nameId);
        }

        public void Unload([NotNull] WurmTimer timer)
        {
            if (timer == null) throw new ArgumentNullException("timer");
            persistentObjectResolver.Unload(timer);
        }

        protected virtual void OnCustomTimerRemoved(CustomTimerRemovedEventArgs e)
        {
            var handler = CustomTimerRemoved;
            if (handler != null) handler(this, e);
        }

        public void ThrowIfNoDefinition([NotNull] TimerDefinition timerDefinition)
        {
            if (timerDefinition == null) throw new ArgumentNullException("timerDefinition");
            if (!allDefinitions.Contains(timerDefinition))
            {
                throw new InvalidOperationException("No descriptor found for timerDefintion " + timerDefinition.ToDebugString());
            }
        }

        public IEnumerable GetNamesOfAllCustomTimers()
        {
            return customDefinitions.Select(definition => definition.TimerDefinitionId.Name).Distinct().ToArray();
        }
    }

    public class CustomTimerRemovedEventArgs : EventArgs
    {
        public string NameId { get; private set; }
        public CustomTimerRemovedEventArgs(string nameId)
        {
            NameId = nameId;
        }
    }
}