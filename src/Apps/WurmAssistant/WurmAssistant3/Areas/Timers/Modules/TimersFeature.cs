using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using AldursLab.PersistentObjects;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Areas.Core.Contracts;
using AldursLab.WurmAssistant3.Areas.Features.Contracts;
using AldursLab.WurmAssistant3.Areas.Logging.Contracts;
using AldursLab.WurmAssistant3.Areas.Persistence.Contracts;
using AldursLab.WurmAssistant3.Areas.SoundManager.Contracts;
using AldursLab.WurmAssistant3.Areas.Timers.Views;
using AldursLab.WurmAssistant3.Areas.Timers.Views.Timers;
using AldursLab.WurmAssistant3.Areas.TrayPopups.Contracts;
using AldursLab.WurmAssistant3.Properties;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Ninject;
using WurmAssistantDataTransfer.Dtos;

namespace AldursLab.WurmAssistant3.Areas.Timers.Modules
{
    [PersistentObject("TimersFeature")]
    public sealed class TimersFeature : PersistentObjectBase, IFeature, IInitializable, IDisposable
    {
        readonly ILogger logger;
        readonly IWurmApi wurmApi;
        readonly ISoundManager soundManager;
        readonly ITrayPopups trayPopups;
        readonly TimerDefinitions timerDefinitions;
        readonly IPersistentObjectResolver<PlayerTimersGroup> playerTimersGroupsResolver;
        readonly TimerInstances timerInstances;

        readonly List<PlayerTimersGroup> timerGroups = new List<PlayerTimersGroup>();

        [JsonProperty] 
        readonly HashSet<Guid> currentGroupsIds = new HashSet<Guid>();
        [JsonProperty]
        Point savedWindowSize;
        [JsonProperty]
        bool widgetModeEnabled;
        [JsonProperty]
        Color widgetBgColor = SystemColors.Control;
        [JsonProperty]
        Color widgetForeColor = SystemColors.ControlText;
        [JsonProperty] 
        bool showEndDate = false;
        [JsonProperty] 
        bool showEndDateInsteadOfTimeRemaining = false;

        TimersForm timersForm;
        readonly ITimer updateTimer;

        public TimersFeature(
            [NotNull] ITimerFactory timerFactory, 
            [NotNull] ILogger logger,
            [NotNull] IWurmApi wurmApi, 
            [NotNull] ISoundManager soundManager, 
            [NotNull] ITrayPopups trayPopups,
            [NotNull] TimerDefinitions timerDefinitions,
            [NotNull] IPersistentObjectResolver<PlayerTimersGroup> playerTimersGroupsResolver,
            [NotNull] TimerInstances timerInstances)
        {
            if (timerFactory == null) throw new ArgumentNullException(nameof(timerFactory));
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            if (wurmApi == null) throw new ArgumentNullException(nameof(wurmApi));
            if (soundManager == null) throw new ArgumentNullException(nameof(soundManager));
            if (trayPopups == null) throw new ArgumentNullException(nameof(trayPopups));
            if (timerDefinitions == null) throw new ArgumentNullException(nameof(timerDefinitions));
            if (playerTimersGroupsResolver == null) throw new ArgumentNullException(nameof(playerTimersGroupsResolver));
            if (timerInstances == null) throw new ArgumentNullException(nameof(timerInstances));
            this.logger = logger;
            this.wurmApi = wurmApi;
            this.soundManager = soundManager;
            this.trayPopups = trayPopups;
            this.timerDefinitions = timerDefinitions;
            this.playerTimersGroupsResolver = playerTimersGroupsResolver;
            this.timerInstances = timerInstances;

            updateTimer = timerFactory.CreateUiThreadTimer();

            updateTimer.Interval = TimeSpan.FromMilliseconds(500);
            updateTimer.Tick += (sender, args) =>
            {
                foreach (var timergroup in timerGroups)
                {
                    timergroup.Update();
                };
            };
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

        public bool ShowEndDate
        {
            get { return showEndDate; }
            set { showEndDate = value; FlagAsChanged(); }
        }

        public bool ShowEndDateInsteadOfTimeRemaining
        {
            get { return showEndDateInsteadOfTimeRemaining; }
            set { showEndDateInsteadOfTimeRemaining = value; FlagAsChanged(); }
        }

        public void Initialize()
        {
            timerDefinitions.CustomTimerRemoved += TimerDefinitionsRemovedCustomTimer;
            timersForm = new TimersForm(this, logger, wurmApi, timerDefinitions);
            foreach (Guid groupId in currentGroupsIds)
            {
                try
                {
                    RestoreGroup(groupId);
                }
                catch (Exception exception)
                {
                    logger.Error(exception, "Error during initialization of server group id " + groupId);
                }
            }

            updateTimer.Start();
        }

        void TimerDefinitionsRemovedCustomTimer(object sender, CustomTimerRemovedEventArgs e)
        {
            foreach (var timergroup in timerGroups)
            {
                timergroup.StopAndRemoveMatchingTimerDefinition(e.DefinitionId);
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

        internal IEnumerable<PlayerTimersGroup> GetActivePlayerGroups()
        {
            return timerGroups.ToArray();
        }

        internal void CreateGroup(Guid groupId, string characterName, string serverGroupId)
        {
            var group = new PlayerTimersGroup(groupId.ToString(),
                            this,
                            wurmApi,
                            logger,
                            soundManager,
                            trayPopups,
                            timerDefinitions,
                            timerInstances);
            playerTimersGroupsResolver.LoadAndStartTracking(group);

            group.CharacterName = characterName;
            group.ServerGroupId = serverGroupId;

            group.Initialize();

            timerGroups.Add(group);
            currentGroupsIds.Add(groupId);

            FlagAsChanged();
        }

        internal void RestoreGroup(Guid groupId)
        {
            var group = new PlayerTimersGroup(groupId.ToString(),
                this,
                wurmApi,
                logger,
                soundManager,
                trayPopups,
                timerDefinitions,
                timerInstances);
            playerTimersGroupsResolver.LoadAndStartTracking(group);
            try
            {
                group.Initialize();
            }
            finally 
            {
                if (!timerGroups.Contains(group)) timerGroups.Add(group);
                currentGroupsIds.Add(groupId);

                FlagAsChanged();
            }
        }

        internal void RemovePlayerGroup(Guid groupId)
        {
            var group = timerGroups.First(x => x.Id == groupId);
            group.Stop();
            group.RemoveAllTimers();
            playerTimersGroupsResolver.UnloadAndDeleteData(group);
            timerGroups.Remove(group);
            currentGroupsIds.Remove(groupId);

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

        public void PopulateDto(WurmAssistantDto dto)
        {
        }

        public async Task ImportDataFromWa2Async(WurmAssistantDto dto)
        {
            TimersWa2Importer importer = new TimersWa2Importer(this, timerDefinitions, soundManager, logger);
            await importer.ImportFromDtoAsync(dto);
        }

        public int DataImportOrder { get { return 0; } }

        #endregion IFeature

        public void IncreaseSortingOrder(PlayerTimersGroup selectedGroup)
        {
            var list = timerGroups.OrderBy(@group => @group.SortingOrder).ToList();
            var index = list.IndexOf(selectedGroup);
            if (index >= 0)
            {
                var prevIndex = index - 1;
                if (prevIndex >= 0)
                {
                    var prevItem = list[prevIndex];
                    list[prevIndex] = list[index];
                    list[index] = prevItem;
                    for (int i = 0; i < list.Count; i++)
                    {
                        list[i].SortingOrder = i;
                    }
                }
            }
        }

        public void ReduceSortingOrder(PlayerTimersGroup selectedGroup)
        {
            var list = timerGroups.OrderBy(@group => @group.SortingOrder).ToList();
            var index = list.IndexOf(selectedGroup);
            if (index >= 0)
            {
                var nextIndex = index + 1;
                if (nextIndex <= list.Count - 1)
                {
                    var nextItem = list[nextIndex];
                    list[nextIndex] = list[index];
                    list[index] = nextItem;
                    for (int i = 0; i < list.Count; i++)
                    {
                        list[i].SortingOrder = i;
                    }
                }
            }
        }

        public void Dispose()
        {
            updateTimer.Stop();
            foreach (var timergroup in timerGroups)
            {
                timergroup.Stop();
            }
            timersForm.Dispose();
        }
    }
}
