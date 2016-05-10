using System;
using System.Collections.Generic;
using System.Linq;
using AldursLab.Essentials.Extensions.DotNet.Collections.Generic;
using AldursLab.PersistentObjects;
using AldursLab.WurmAssistant3.Areas.Timers.Contracts;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Ninject;

namespace AldursLab.WurmAssistant3.Areas.Timers.Modules
{
    [PersistentObject("TimersFeature_WurmTimerDescriptors")]
    public class TimerDefinitions : PersistentObjectBase, IInitializable
    {
        [JsonProperty] 
        readonly Dictionary<Guid, TimerDefinition> timerDefinitions = new Dictionary<Guid, TimerDefinition>();

        public event EventHandler<CustomTimerRemovedEventArgs> CustomTimerRemoved;

        public void Initialize()
        {
            OverwriteDefaultDefinition(new TimerDefinition(new Guid("a420f74d-4c3c-474a-89ab-477caf501d8c"))
            {
                Name = "Meditation",
                RuntimeTypeId = RuntimeTypeId.Meditation
            });
            OverwriteDefaultDefinition(new TimerDefinition(new Guid("eb0df9dd-f91c-4dad-8aa0-44bf2243d2a2"))
            {
                Name = "Path Question",
                RuntimeTypeId = RuntimeTypeId.MeditPath
            });
            OverwriteDefaultDefinition(new TimerDefinition(new Guid("52caa2c2-fa72-4d57-8887-d06b8b1db898"))
            {
                Name = "Prayer",
                RuntimeTypeId = RuntimeTypeId.Prayer
            });
            OverwriteDefaultDefinition(new TimerDefinition(new Guid("d080c694-f1d1-43d9-b1b2-6edf0147fbf3"))
            {
                Name = "Sermon",
                RuntimeTypeId = RuntimeTypeId.Sermon
            });
            OverwriteDefaultDefinition(new TimerDefinition(new Guid("6e989db6-d49a-4f1d-956a-a83f4192b058"))
            {
                Name = "Alignment",
                RuntimeTypeId = RuntimeTypeId.Alignment
            });
            OverwriteDefaultDefinition(new TimerDefinition(new Guid("f1238677-3139-41ba-bbee-3b61186a11ab"))
            {
                Name = "Junk Sale",
                RuntimeTypeId = RuntimeTypeId.JunkSale
            });
        }

        void OverwriteDefaultDefinition(TimerDefinition timerDefinition)
        {
            timerDefinitions[timerDefinition.Id] = timerDefinition;
        }

        public void AddTimerDefinition([NotNull] TimerDefinition timerDefinition)
        {
            if (timerDefinition == null) throw new ArgumentNullException("timerDefinition");
            if (timerDefinitions.ContainsKey(timerDefinition.Id))
            {
                throw new ApplicationException(string.Format((string) "Timer with id {0} already registered.", timerDefinition.Id));
            }
            timerDefinitions[timerDefinition.Id] = timerDefinition;
            FlagAsChanged();
        }

        public void AddTimerDefinitionIfNotExists([NotNull] TimerDefinition timerDefinition)
        {
            if (!timerDefinitions.ContainsKey(timerDefinition.Id))
            {
                AddTimerDefinition(timerDefinition);
            }
            FlagAsChanged();
        }

        public void RemoveCustomTimerDefinition(Guid id)
        {
            bool removed = timerDefinitions.Remove(id);
            FlagAsChanged();
            if (removed) OnCustomTimerRemoved(new CustomTimerRemovedEventArgs(id));
        }

        [CanBeNull]
        public CustomTimerDefinition TryGetOptionsTemplateForCustomTimer(Guid id)
        {
            return timerDefinitions[id].CustomTimerConfig;
        }

        protected virtual void OnCustomTimerRemoved(CustomTimerRemovedEventArgs e)
        {
            var handler = CustomTimerRemoved;
            if (handler != null) handler(this, e);
        }

        public IEnumerable<TimerDefinition> GetCustomTimerDefinitions()
        {
            return timerDefinitions.Values.Where(definition => definition.RuntimeTypeId == RuntimeTypeId.LegacyCustom).ToArray();
        }

        public IEnumerable<TimerDefinition> GetDefinitionsOfUnusedTimers(IEnumerable<Guid> existingDefinitionIds)
        {
            var hashset = existingDefinitionIds.ToHashSet();
            return timerDefinitions.Values.Where(definition => !hashset.Contains(definition.Id)).ToArray();
        }

        public TimerDefinition GetById(Guid definitionId)
        {
            return timerDefinitions[definitionId];
        }

        public TimerDefinition TryGetById(Guid definitionId)
        {
            TimerDefinition result;
            timerDefinitions.TryGetValue(definitionId, out result);
            return result;
        }

        public TimerDefinition TryGetByName(string name)
        {
            return timerDefinitions.Values.SingleOrDefault(
                definition => definition.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        }
    }

    public class CustomTimerRemovedEventArgs : EventArgs
    {
        public Guid DefinitionId { get; private set; }
        public CustomTimerRemovedEventArgs(Guid definitionId)
        {
            DefinitionId = definitionId;
        }
    }
}