using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AldursLab.WurmAssistant3.Areas.Triggers.Data.Migrations.V1Data;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.Triggers.Data.Migrations
{
    [KernelBind(BindingHint.Transient), UsedImplicitly]
    class TriggersV2Migration
    {
        readonly ObsoleteDataProvider obsoleteDataProvider;
        readonly TriggersDataContext triggersDataContext;

        public TriggersV2Migration(
            [NotNull] ObsoleteDataProvider obsoleteDataProvider,
            [NotNull] TriggersDataContext triggersDataContext)
        {
            if (obsoleteDataProvider == null) throw new ArgumentNullException(nameof(obsoleteDataProvider));
            if (triggersDataContext == null) throw new ArgumentNullException(nameof(triggersDataContext));
            this.obsoleteDataProvider = obsoleteDataProvider;
            this.triggersDataContext = triggersDataContext;
        }

        public void Run()
        {
            MigrateActionQueueTriggerConditions();
            MigrateActivePlayersConfigs();
        }

        void MigrateActionQueueTriggerConditions()
        {
            foreach (var condition in obsoleteDataProvider.Cmo.allConditions)
            {
                triggersDataContext.ActionQueueTriggerConfig.Conditions.Add(condition.Key, condition.Value);
            }
        }

        void MigrateActivePlayersConfigs()
        {
            foreach (var characterName in obsoleteDataProvider.Tfo.activeCharacterNames)
            {
                triggersDataContext.TriggersConfig.ActiveCharacterNames.Add(characterName);

                ActiveTriggersObj ato;
                obsoleteDataProvider.Atos.TryGetValue(characterName, out ato);

                TriggerManagerObj tmo;
                obsoleteDataProvider.Tmos.TryGetValue(characterName, out tmo);

                var item = triggersDataContext.CharacterTriggersConfigs.GetOrCreate(characterName);
                item.Muted = tmo?.muted ?? false;
                item.TriggerListState = tmo?.TriggerListState ?? new byte[0];
                if (ato != null)
                {
                    foreach (var kvp in ato.triggerDatas)
                    {
                        item.TriggerEntities.Add(kvp.Key, kvp.Value);
                    }
                }
            }
        }
    }
}
