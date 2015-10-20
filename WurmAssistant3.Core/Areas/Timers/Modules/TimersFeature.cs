using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using AldursLab.PersistentObjects;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Core.Areas.Features.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.Logging.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.Persistence.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.SoundEngine.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.Timers.Views;
using AldursLab.WurmAssistant3.Core.Areas.Timers.Views.Timers;
using AldursLab.WurmAssistant3.Core.Areas.TrayPopups.Contracts;
using AldursLab.WurmAssistant3.Core.Properties;
using AldursLab.WurmAssistant3.Core.Root.Contracts;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Ninject;

namespace AldursLab.WurmAssistant3.Core.Areas.Timers.Modules
{
    [PersistentObject("TimersFeature")]
    public sealed class TimersFeature : PersistentObjectBase, IFeature, IInitializable
    {
        readonly IHostEnvironment host;
        readonly ILogger logger;
        readonly IWurmApi wurmApi;
        readonly ISoundEngine soundEngine;
        readonly ITrayPopups trayPopups;
        readonly TimerDefinitions timerDefinitions;
        readonly IPersistentObjectResolver<PlayerTimersGroup> playerTimersGroupsResolver;

        readonly List<PlayerTimersGroup> timerGroups = new List<PlayerTimersGroup>();

        [JsonProperty]
        HashSet<string> activePlayers = new HashSet<string>();
        [JsonProperty]
        Point savedWindowSize;
        [JsonProperty]
        bool widgetModeEnabled;
        [JsonProperty]
        Color widgetBgColor = SystemColors.Control;
        [JsonProperty]
        Color widgetForeColor = SystemColors.ControlText;

        TimersForm timersForm;

        public TimersFeature([NotNull] IHostEnvironment host, IUpdateLoop updateLoop, [NotNull] ILogger logger,
            [NotNull] IWurmApi wurmApi, [NotNull] ISoundEngine soundEngine, [NotNull] ITrayPopups trayPopups,
            [NotNull] TimerDefinitions timerDefinitions,
            [NotNull] IPersistentObjectResolver<PlayerTimersGroup> playerTimersGroupsResolver)
        {
            if (host == null) throw new ArgumentNullException("host");
            if (logger == null) throw new ArgumentNullException("logger");
            if (wurmApi == null) throw new ArgumentNullException("wurmApi");
            if (soundEngine == null) throw new ArgumentNullException("soundEngine");
            if (trayPopups == null) throw new ArgumentNullException("trayPopups");
            if (timerDefinitions == null) throw new ArgumentNullException("timerDefinitions");
            if (playerTimersGroupsResolver == null) throw new ArgumentNullException("playerTimersGroupsResolver");
            this.host = host;
            this.logger = logger;
            this.wurmApi = wurmApi;
            this.soundEngine = soundEngine;
            this.trayPopups = trayPopups;
            this.timerDefinitions = timerDefinitions;
            this.playerTimersGroupsResolver = playerTimersGroupsResolver;

            host.HostClosing += (sender, args) =>
            {
                foreach (var timergroup in timerGroups)
                {
                    timergroup.Stop();
                }
                timersForm.Dispose();
            };

            updateLoop.Updated += (sender, args) =>
            {
                foreach (var timergroup in timerGroups)
                {
                    timergroup.Update();
                };
            };
        }

        public HashSet<string> ActivePlayers
        {
            get { return activePlayers; }
            set { activePlayers = value; FlagAsChanged(); }
        }

        public Point SavedWindowSize
        {
            get { return savedWindowSize; }
            set { savedWindowSize = value; FlagAsChanged(); }
        }

        public bool WidgetModeEnabled
        {
            get { return widgetModeEnabled; }
            set { widgetModeEnabled = value; FlagAsChanged(); }
        }

        public Color WidgetBgColor
        {
            get { return widgetBgColor; }
            set { widgetBgColor = value; FlagAsChanged(); }
        }

        public Color WidgetForeColor
        {
            get { return widgetForeColor; }
            set { widgetForeColor = value; FlagAsChanged(); }
        }


        public void Initialize()
        {
            timerDefinitions.CustomTimerRemoved += TimerDefinitionsRemovedCustomTimer;
            timersForm = new TimersForm(this, logger, wurmApi, timerDefinitions);
            foreach (string player in ActivePlayers)
            {
                AddNewPlayerGroup(player);
            }
        }

        void TimerDefinitionsRemovedCustomTimer(object sender, CustomTimerRemovedEventArgs e)
        {
            foreach (var timergroup in timerGroups)
            {
                timergroup.RemoveDeletedCustomTimer(e.NameId);
            }
        }

        internal void RegisterTimersGroup(PlayerLayoutView layoutControl)
        {
            timersForm.RegisterTimersGroup(layoutControl);
        }

        internal void UnregisterTimersGroup(PlayerLayoutView layoutControl)
        {
            timersForm.UnregisterTimersGroup(layoutControl);
        }

        internal string[] GetActivePlayerGroups()
        {
            var result = new List<string>();
            foreach (var name in ActivePlayers)
            {
                result.Add(name);
            }
            return result.ToArray();
        }

        internal void AddNewPlayerGroup(string player)
        {
            var group = new PlayerTimersGroup(this,
                player,
                wurmApi,
                logger,
                soundEngine,
                trayPopups,
                timerDefinitions);
            playerTimersGroupsResolver.StartTracking(group);
            group.Initialize();
            timerGroups.Add(group);
            ActivePlayers.Add(player);
            FlagAsChanged();
        }

        internal void RemovePlayerGroup(string player)
        {
            var group = timerGroups.First(x => x.CharacterName == player);
            group.Stop();
            playerTimersGroupsResolver.Unload(group);
            timerGroups.Remove(group);
            ActivePlayers.Remove(player);
            FlagAsChanged();
        }

        internal TimersForm GetModuleUi()
        {
            return this.timersForm;
        }

        #region IFeature

        void IFeature.Show()
        {
            timersForm.ShowAndBringToFront();
        }

        void IFeature.Hide()
        {
            timersForm.Hide();
        }

        string IFeature.Name
        {
            get { return "Timers"; }
        }

        Image IFeature.Icon
        {
            get { return Resources.TimersIcon; }
        }

        async Task IFeature.InitAsync()
        {
            // no async inits required
            await Task.FromResult(true);
        }

        #endregion IFeature
    }
}
