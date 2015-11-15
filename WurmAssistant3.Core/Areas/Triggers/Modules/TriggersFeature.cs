using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using AldursLab.PersistentObjects;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Core.Areas.Features.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.Logging.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.Persistence.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.SoundEngine.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.TrayPopups.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.Triggers.Views;
using AldursLab.WurmAssistant3.Core.Properties;
using AldursLab.WurmAssistant3.Core.Root.Contracts;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Ninject;
using WurmAssistantDataTransfer.Dtos;

namespace AldursLab.WurmAssistant3.Core.Areas.Triggers.Modules
{
    [PersistentObject("TriggersFeature")]
    public class TriggersFeature : PersistentObjectBase, IFeature, IInitializable
    {
        readonly ISoundEngine soundEngine;
        readonly IWurmAssistantDataDirectory wurmAssistantDataDirectory;
        readonly IUpdateLoop updateLoop;
        readonly IHostEnvironment hostEnvironment;
        readonly IWurmApi wurmApi;
        readonly IPersistentObjectResolver<TriggerManager> triggerManagerResolver;
        private readonly ITrayPopups trayPopups;
        private readonly ILogger logger;

        [JsonProperty]
        readonly HashSet<string> activeCharacterNames = new HashSet<string>();

        FormTriggersMain mainUi;
        readonly Dictionary<string, TriggerManager> triggerManagers = new Dictionary<string, TriggerManager>();

        public TriggersFeature([NotNull] ISoundEngine soundEngine,
            [NotNull] IWurmAssistantDataDirectory wurmAssistantDataDirectory, [NotNull] IUpdateLoop updateLoop,
            [NotNull] IHostEnvironment hostEnvironment, [NotNull] IWurmApi wurmApi,
            [NotNull] IPersistentObjectResolver<TriggerManager> triggerManagerResolver, [NotNull] ITrayPopups trayPopups,
            [NotNull] ILogger logger)
        {
            if (soundEngine == null) throw new ArgumentNullException("soundEngine");
            if (wurmAssistantDataDirectory == null) throw new ArgumentNullException("wurmAssistantDataDirectory");
            if (updateLoop == null) throw new ArgumentNullException("updateLoop");
            if (hostEnvironment == null) throw new ArgumentNullException("hostEnvironment");
            if (wurmApi == null) throw new ArgumentNullException("wurmApi");
            if (triggerManagerResolver == null) throw new ArgumentNullException("triggerManagerResolver");
            if (trayPopups == null) throw new ArgumentNullException("trayPopups");
            if (logger == null) throw new ArgumentNullException("logger");
            this.soundEngine = soundEngine;
            this.wurmAssistantDataDirectory = wurmAssistantDataDirectory;
            this.updateLoop = updateLoop;
            this.hostEnvironment = hostEnvironment;
            this.wurmApi = wurmApi;
            this.triggerManagerResolver = triggerManagerResolver;
            this.trayPopups = trayPopups;
            this.logger = logger;

            updateLoop.Updated += (sender, args) => Update();
            hostEnvironment.HostClosing += (sender, args) => Stop();
        }

        public void Initialize()
        {
            mainUi = new FormTriggersMain(this, soundEngine);
            foreach (var name in GetAllActiveCharacters())
            {
                AddManager(name);
            }
        }

        public void AddNewNotifier()
        {
            string[] allPlayers = wurmApi.Characters.All.Select(character => character.Name.Capitalized).ToArray();
            var ui = new FormChoosePlayer(allPlayers.Where(player => !triggerManagers.ContainsKey(player)).ToArray());
            if (ui.ShowDialog() == DialogResult.OK)
            {
                string[] results = ui.result;
                foreach (var name in results)
                {
                    AddManager(name);
                }
            }
        }

        public void RemoveManager(TriggerManager notifier)
        {
            triggerManagers.Remove(notifier.CharacterName);
            RemoveActiveCharacter(notifier.CharacterName);
            FlagAsChanged();
            mainUi.RemoveNotifierController(notifier.GetUIHandle());
        }


        private void AddActiveCharacter(string characterName)
        {
            activeCharacterNames.Add(characterName);
            FlagAsChanged();
        }

        private void RemoveActiveCharacter(string characterName)
        {
            activeCharacterNames.Remove(characterName);
            FlagAsChanged();
        }

        private ICollection<string> GetAllActiveCharacters()
        {
            return activeCharacterNames.ToArray();
        }

        private void Update()
        {
            foreach (var notifier in triggerManagers.Values.ToArray())
            {
                notifier.Update();
            }
        }

        private void Stop()
        {
            // do not clean notifiers, they clean themselves on host closing.
        }

        private void AddManager(string charName)
        {
            try
            {
                var triggerManager = triggerManagerResolver.Get(charName);
                AddManager(triggerManager);
            }
            catch (Exception exception)
            {
                logger.Error(exception, "Failed to create triggers manager for character " + charName);
            }
        }

        private void AddManager(TriggerManager triggerManager)
        {
            triggerManager.TriggersFeature = this;
            mainUi.AddNotifierController(triggerManager.GetUIHandle());
            triggerManagers.Add(triggerManager.CharacterName, triggerManager);
            AddActiveCharacter(triggerManager.CharacterName);
            FlagAsChanged();
        }

        #region IFeature

        void IFeature.Show()
        {
            mainUi.ShowAndBringToFront();
        }

        void IFeature.Hide()
        {
        }

        string IFeature.Name
        {
            get { return "Triggers"; }
        }

        Image IFeature.Icon
        {
            get { return Resources.TriggersIcon; }
        }

        async Task IFeature.InitAsync()
        {
            await Task.Delay(0);
        }

        public void PopulateDto(WurmAssistantDto dto)
        {
        }

        public async Task ImportDataFromWa2Async(WurmAssistantDto dto)
        {
            TriggersWa2Importer importer = new TriggersWa2Importer(soundEngine, trayPopups, triggerManagers, logger);
            await importer.ImportFromDtoAsync(dto);
        }

        public int DataImportOrder { get { return 0; } }

        #endregion
    }
}
