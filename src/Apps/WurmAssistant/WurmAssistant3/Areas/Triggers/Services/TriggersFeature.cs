using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using AldursLab.PersistentObjects;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Areas.Core.Contracts;
using AldursLab.WurmAssistant3.Areas.Features.Contracts;
using AldursLab.WurmAssistant3.Areas.Logging.Contracts;
using AldursLab.WurmAssistant3.Areas.Persistence.Contracts;
using AldursLab.WurmAssistant3.Areas.SoundManager.Contracts;
using AldursLab.WurmAssistant3.Areas.TrayPopups.Contracts;
using AldursLab.WurmAssistant3.Areas.Triggers.Parts;
using AldursLab.WurmAssistant3.Properties;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Ninject;

namespace AldursLab.WurmAssistant3.Areas.Triggers.Services
{
    [KernelBind(BindingHint.Singleton), PersistentObject("TriggersFeature")]
    public class TriggersFeature : PersistentObjectBase, IFeature, IInitializable, IDisposable
    {
        readonly ISoundManager soundManager;
        readonly IWurmAssistantDataDirectory wurmAssistantDataDirectory;
        readonly IWurmApi wurmApi;
        readonly IPersistentObjectResolver<TriggerManager> triggerManagerResolver;
        readonly ITrayPopups trayPopups;
        readonly ILogger logger;
        readonly ITimer updateTimer;

        [JsonProperty]
        readonly HashSet<string> activeCharacterNames = new HashSet<string>();

        FormTriggersMain mainUi;
        readonly Dictionary<string, TriggerManager> triggerManagers = new Dictionary<string, TriggerManager>();

        public TriggersFeature(
            [NotNull] ISoundManager soundManager,
            [NotNull] IWurmAssistantDataDirectory wurmAssistantDataDirectory,
            [NotNull] ITimerFactory timerFactory,
            [NotNull] IWurmApi wurmApi,
            [NotNull] IPersistentObjectResolver<TriggerManager> triggerManagerResolver, 
            [NotNull] ITrayPopups trayPopups,
            [NotNull] ILogger logger)
        {
            if (soundManager == null) throw new ArgumentNullException(nameof(soundManager));
            if (wurmAssistantDataDirectory == null) throw new ArgumentNullException(nameof(wurmAssistantDataDirectory));
            if (timerFactory == null) throw new ArgumentNullException(nameof(timerFactory));
            if (wurmApi == null) throw new ArgumentNullException(nameof(wurmApi));
            if (triggerManagerResolver == null) throw new ArgumentNullException(nameof(triggerManagerResolver));
            if (trayPopups == null) throw new ArgumentNullException(nameof(trayPopups));
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            this.soundManager = soundManager;
            this.wurmAssistantDataDirectory = wurmAssistantDataDirectory;
            this.wurmApi = wurmApi;
            this.triggerManagerResolver = triggerManagerResolver;
            this.trayPopups = trayPopups;
            this.logger = logger;

            updateTimer = timerFactory.CreateUiThreadTimer();
            updateTimer.Interval = TimeSpan.FromMilliseconds(500);
            updateTimer.Tick += (sender, args) => Update();
        }

        public void Initialize()
        {
            mainUi = new FormTriggersMain(this, soundManager);
            foreach (var name in GetAllActiveCharacters())
            {
                AddManager(name);
            }
            updateTimer.Start();
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

        #endregion

        public void Dispose()
        {
            updateTimer.Stop();
            foreach (var triggerManager in triggerManagers)
            {
                triggerManager.Value.Dispose();
            }
        }
    }
}
