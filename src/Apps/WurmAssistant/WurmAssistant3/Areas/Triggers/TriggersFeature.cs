using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using AldursLab.PersistentObjects;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Areas.Core;
using AldursLab.WurmAssistant3.Areas.Features;
using AldursLab.WurmAssistant3.Areas.Logging;
using AldursLab.WurmAssistant3.Areas.Persistence;
using AldursLab.WurmAssistant3.Areas.SoundManager;
using AldursLab.WurmAssistant3.Areas.TrayPopups;
using AldursLab.WurmAssistant3.Areas.Triggers.Data;
using AldursLab.WurmAssistant3.Areas.Triggers.Factories;
using AldursLab.WurmAssistant3.Properties;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Ninject;

namespace AldursLab.WurmAssistant3.Areas.Triggers
{
    [KernelBind(BindingHint.Singleton)]
    public class TriggersFeature : IFeature, IDisposable
    {
        readonly ISoundManager soundManager;
        readonly IWurmAssistantDataDirectory wurmAssistantDataDirectory;
        readonly IWurmApi wurmApi;
        readonly ITrayPopups trayPopups;
        readonly ILogger logger;
        readonly TriggersDataContext triggersDataContext;
        readonly ITriggerManagerFactory triggerManagerFactory;
        readonly ITimer updateTimer;

        readonly FormTriggersMain mainUi;
        readonly Dictionary<string, TriggerManager> triggerManagers = new Dictionary<string, TriggerManager>();

        public TriggersFeature(
            [NotNull] ISoundManager soundManager,
            [NotNull] IWurmAssistantDataDirectory wurmAssistantDataDirectory,
            [NotNull] ITimerFactory timerFactory,
            [NotNull] IWurmApi wurmApi,
            [NotNull] ITrayPopups trayPopups,
            [NotNull] ILogger logger,
            [NotNull] TriggersDataContext triggersDataContext,
            [NotNull] ITriggerManagerFactory triggerManagerFactory)
        {
            if (soundManager == null) throw new ArgumentNullException(nameof(soundManager));
            if (wurmAssistantDataDirectory == null) throw new ArgumentNullException(nameof(wurmAssistantDataDirectory));
            if (timerFactory == null) throw new ArgumentNullException(nameof(timerFactory));
            if (wurmApi == null) throw new ArgumentNullException(nameof(wurmApi));
            if (trayPopups == null) throw new ArgumentNullException(nameof(trayPopups));
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            if (triggersDataContext == null) throw new ArgumentNullException(nameof(triggersDataContext));
            if (triggerManagerFactory == null) throw new ArgumentNullException(nameof(triggerManagerFactory));
            this.soundManager = soundManager;
            this.wurmAssistantDataDirectory = wurmAssistantDataDirectory;
            this.wurmApi = wurmApi;
            this.trayPopups = trayPopups;
            this.logger = logger;
            this.triggersDataContext = triggersDataContext;
            this.triggerManagerFactory = triggerManagerFactory;

            updateTimer = timerFactory.CreateUiThreadTimer();
            updateTimer.Interval = TimeSpan.FromMilliseconds(500);
            updateTimer.Tick += (sender, args) => Update();

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
            mainUi.RemoveNotifierController(notifier.GetUiHandle());
        }

        private void AddActiveCharacter(string characterName)
        {
            triggersDataContext.TriggersConfig.ActiveCharacterNames.Add(characterName);
        }

        private void RemoveActiveCharacter(string characterName)
        {
            triggersDataContext.TriggersConfig.ActiveCharacterNames.Remove(characterName);
        }

        private ICollection<string> GetAllActiveCharacters()
        {
            return triggersDataContext.TriggersConfig.ActiveCharacterNames.ToArray();
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
                var triggerManager = triggerManagerFactory.CreateTriggerManager(charName);
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
            mainUi.AddNotifierController(triggerManager.GetUiHandle());
            triggerManagers.Add(triggerManager.CharacterName, triggerManager);
            AddActiveCharacter(triggerManager.CharacterName);
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
