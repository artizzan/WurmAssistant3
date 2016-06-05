using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AldursLab.PersistentObjects;
using AldursLab.WurmApi;
using AldursLab.WurmApi.Modules.Wurm.Characters;
using AldursLab.WurmAssistant3.Areas.Logging.Contracts;
using AldursLab.WurmAssistant3.Areas.SoundManager.Contracts;
using AldursLab.WurmAssistant3.Areas.Timers.Contracts;
using AldursLab.WurmAssistant3.Areas.Timers.Features;
using AldursLab.WurmAssistant3.Areas.Timers.Parts.Timers;
using AldursLab.WurmAssistant3.Areas.Timers.Singletons;
using AldursLab.WurmAssistant3.Areas.TrayPopups.Contracts;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Ninject;

namespace AldursLab.WurmAssistant3.Areas.Timers.Parts
{
    [PersistentObject("TimersFeature_PlayerTimersGroup")]
    public class PlayerTimersGroup : PersistentObjectBase, IInitializable
    {
        [JsonObject(MemberSerialization.OptIn)]
        public class ActiveTimer
        {
            [JsonProperty("definitionId")]
            public Guid DefinitionId { get; set; }
            [JsonProperty("timerId")]
            public Guid TimerId { get; set; }

            public override string ToString()
            {
                return string.Format("DefinitionId: {0}, TimerId: {1}", DefinitionId, TimerId);
            }
        }

        [JsonProperty] 
        readonly List<ActiveTimer> activeTimerDefinitions = new List<ActiveTimer>();

        [JsonProperty]
        string characterName;
        public string CharacterName
        {
            get { return characterName; }
            set { characterName = value; FlagAsChanged(); }
        }

        [JsonProperty]
        string serverGroupId;
        public string ServerGroupId
        {
            get { return serverGroupId; }
            set { serverGroupId = value; FlagAsChanged(); }
        }

        [JsonProperty]
        int sortingOrder;
        public int SortingOrder
        {
            get { return sortingOrder; }
            set { sortingOrder = value; FlagAsChanged(); }
        }

        [JsonProperty()] 
        bool hidden;
        public bool Hidden
        {
            get { return hidden; }
            set
            {
                hidden = value;
                if (hidden)
                {
                    if (timersFeature != null && layoutControl != null)
                    {
                        timersFeature.UnregisterTimersGroup(layoutControl);
                    }
                }
                else
                {
                    if (timersFeature != null && layoutControl != null)
                    {
                        timersFeature.RegisterTimersGroup(layoutControl);
                    }
                }
                FlagAsChanged();
            }
        }

        readonly List<WurmTimer> timers = new List<WurmTimer>();

        PlayerLayoutView layoutControl;
        readonly TimersFeature timersFeature;
        readonly IWurmApi wurmApi;
        readonly ILogger logger;
        readonly TimerDefinitions timerDefinitions;
        readonly TimerInstances timerInstances;

        public TimersFeature TimersFeature { get { return timersFeature; } }

        IWurmCharacter character;
        IWurmServer currentServerOnTheGroup;

        public PlayerTimersGroup(string persistentObjectId, TimersFeature timersFeature, 
            [NotNull] IWurmApi wurmApi, [NotNull] ILogger logger, [NotNull] ISoundManager soundManager, 
            [NotNull] ITrayPopups trayPopups, [NotNull] TimerDefinitions timerDefinitions,
            [NotNull] TimerInstances timerInstances)
            : base(persistentObjectId) 
        {
            if (wurmApi == null) throw new ArgumentNullException("wurmApi");
            if (logger == null) throw new ArgumentNullException("logger");
            if (soundManager == null) throw new ArgumentNullException("soundManager");
            if (trayPopups == null) throw new ArgumentNullException("trayPopups");
            if (timerDefinitions == null) throw new ArgumentNullException("timerDefinitions");
            if (timerInstances == null) throw new ArgumentNullException("timerInstances");
            this.Id = Guid.Parse(persistentObjectId);
            this.timersFeature = timersFeature;
            this.wurmApi = wurmApi;
            this.logger = logger;
            this.timerDefinitions = timerDefinitions;
            this.timerInstances = timerInstances;
        }

        public void Initialize()
        {
            try
            {
                layoutControl = new PlayerLayoutView(this);
                if (!Hidden)
                {
                    this.timersFeature.RegisterTimersGroup(layoutControl);
                }
                wurmApi.LogsMonitor.Subscribe(CharacterName, LogType.AllLogs, OnNewLogEvents);
                character = wurmApi.Characters.Get(CharacterName);
                character.LogInOrCurrentServerPotentiallyChanged += CharacterOnLogInOrCurrentServerPotentiallyChanged;
            }
            catch (Exception)
            {
                if (layoutControl != null)
                {
                    layoutControl.SetInitializationError();
                }
                throw;
            }
            PerformAsyncInits();
        }

        public Guid Id { get; private set; }

        [CanBeNull]
        public IWurmServer CurrentServerOnTheGroup
        {
            get { return currentServerOnTheGroup; }
        }

        public IEnumerable<WurmTimer> Timers
        {
            get { return timers; }
        }

        private async void PerformAsyncInits()
        {
            try
            {
                await AttemptToEstablishCurrentServer(TimeSpan.FromDays(120));

                if (currentServerOnTheGroup == null)
                {
                    await AttemptToEstablishCurrentServer(TimeSpan.FromDays(365));
                }

                foreach (var activeTimer in activeTimerDefinitions)
                {
                    try
                    {
                        var definition = timerDefinitions.GetById(activeTimer.DefinitionId);
                        WurmTimer timer = timerInstances.GetTimer(activeTimer.DefinitionId, activeTimer.TimerId);
                        timer.Initialize(this, CharacterName, definition);
                        layoutControl.RegisterNewTimerDisplay(timer.View);
                        timers.Add(timer);
                        FlagAsChanged();
                    }
                    catch (Exception exception)
                    {
                        logger.Error(exception, "Error at InitTimers for timer " + activeTimer);
                        FlagAsChanged();
                    }
                }

                layoutControl.EnableAddingTimers();
            }
            catch (Exception _e)
            {
                logger.Error(_e, "problem updating current server group");
                if (layoutControl != null)
                {
                    layoutControl.SetInitializationError();
                }
            }
        }

        async Task AttemptToEstablishCurrentServer(TimeSpan timestampToSearch)
        {
            var scanResults =
                await
                    character.Logs.ScanLogsServerGroupRestrictedAsync(DateTime.Now - timestampToSearch,
                        DateTime.Now,
                        LogType.Event,
                        new ServerGroup(ServerGroupId),
                        ScanResultOrdering.Descending);

            foreach (var entry in scanResults)
            {
                var serverStamp = entry.TryGetServerFromLogEntry();
                if (serverStamp != null)
                {
                    var server = wurmApi.Servers.GetByName(serverStamp.ServerName);
                    if (server.ServerGroup.ServerGroupId == ServerGroupId)
                    {
                        currentServerOnTheGroup = server;
                        break;
                    }
                }
            }
        }

        internal void AddNewTimer()
        {
            var availableTimers =
                timerDefinitions.GetDefinitionsOfUnusedTimers(activeTimerDefinitions.Select(timer => timer.DefinitionId));
            
            TimersChoiceForm ui = new TimersChoiceForm(availableTimers, timersFeature.GetModuleUi());
            if (ui.ShowDialogCenteredOnForm(timersFeature.GetModuleUi()) == System.Windows.Forms.DialogResult.OK)
            {
                foreach (var definition in ui.Result)
                {
                    try
                    {
                        AddNewTimer(definition);
                    }
                    catch (Exception exception)
                    {
                        logger.Error(exception, "Error at InitTimers for timer " + definition);
                    }
                }
            }
        }

        public WurmTimer AddNewTimer(TimerDefinition definition)
        {
            WurmTimer newTimer = timerInstances.CreateTimer(definition.Id);
            newTimer.Initialize(this, CharacterName, definition);
            activeTimerDefinitions.Add(new ActiveTimer()
            {
                DefinitionId = definition.Id,
                TimerId = newTimer.Id
            });
            layoutControl.RegisterNewTimerDisplay(newTimer.View);
            timers.Add(newTimer);
            FlagAsChanged();
            return newTimer;
        }

        public void Stop()
        {
            wurmApi.LogsMonitor.Unsubscribe(CharacterName, OnNewLogEvents);

            timersFeature.UnregisterTimersGroup(layoutControl);
            if (character != null)
            {
                character.LogInOrCurrentServerPotentiallyChanged -= CharacterOnLogInOrCurrentServerPotentiallyChanged;
            }
            foreach (var timer in timers)
            {
                timer.Stop();
            }
            layoutControl.Dispose();
        }

        internal void RemoveTimer(WurmTimer timer)
        {
            timerInstances.UnloadAndDeleteTimer(timer);
            timers.Remove(timer);

            var toRemove =
                activeTimerDefinitions.Where(activeTimer => activeTimer.DefinitionId == timer.TimerDefinition.Id).ToArray();
            foreach (var activeTimer in toRemove)
            {
                activeTimerDefinitions.Remove(activeTimer);
            }

            FlagAsChanged();

            timer.Stop();
        }

        public void StopTimer(WurmTimer wurmTimer)
        {
            layoutControl.UnregisterTimerDisplay(wurmTimer.View);
        }

        internal void Update()
        {
            foreach (var timer in timers)
            {
                if (timer.InitCompleted) timer.Update();
                else if (timer.RunUpdateRegardlessOfInitCompleted) timer.Update();
            }
        }

        void CharacterOnLogInOrCurrentServerPotentiallyChanged(object sender, PotentialServerChangeEventArgs potentialServerChangeEventArgs)
        {
            currentServerOnTheGroup = wurmApi.Servers.GetByName(potentialServerChangeEventArgs.ServerName);
        }

        void OnNewLogEvents(object sender, LogsMonitorEventArgs e)
        {
            try
            {
                var currentServer = currentServerOnTheGroup;
                if (currentServer != null && currentServer.ServerGroup.ServerGroupId == ServerGroupId)
                {
                    if (e.LogType == LogType.Event)
                    {
                        foreach (var timer in timers)
                        {
                            if (timer.InitCompleted)
                            {
                                foreach (var entry in e.WurmLogEntries)
                                {
                                    timer.HandleNewEventLogLine(entry);
                                }
                            }
                        }
                    }
                    if (e.LogType == LogType.Skills)
                    {
                        //call all timers with wurmskill handler
                        foreach (var timer in timers)
                        {
                            if (timer.InitCompleted)
                            {
                                foreach (var line in e.WurmLogEntries)
                                {
                                    timer.HandleNewSkillLogLine(line);
                                }
                            }
                        }
                    }

                    foreach (var timer in timers)
                    {
                        if (timer.InitCompleted)
                        {
                            timer.HandleAnyLogLine(e);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                logger.Error(exception, "Error during OnNewLogEvents at timers group " + this.ToString());
            }
        }

        internal TimersForm GetModuleUI()
        {
            return timersFeature.GetModuleUi();
        }

        public void StopAndRemoveMatchingTimerDefinition(Guid definitionId)
        {
            var matching = timers.Where(timer => timer.TimerDefinition.Id == definitionId).ToArray();
            foreach (var timer in matching)
            {
                RemoveTimer(timer);
            }
        }

        public override string ToString()
        {
            return string.Format("{0} ({1}){2}", CharacterName, ServerGroupId, Hidden ? " (hidden)" : string.Empty);
        }

        public void RemoveAllTimers()
        {
            foreach (var wurmTimer in timers.ToArray())
            {
                RemoveTimer(wurmTimer);
            }
        }
    }
}
