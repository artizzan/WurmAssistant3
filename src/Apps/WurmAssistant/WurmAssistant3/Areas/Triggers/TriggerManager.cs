using System;
using System.Collections.Generic;
using System.Linq;
using AldursLab.PersistentObjects;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Areas.Logging;
using AldursLab.WurmAssistant3.Areas.Persistence;
using AldursLab.WurmAssistant3.Areas.SoundManager;
using AldursLab.WurmAssistant3.Areas.TrayPopups;
using AldursLab.WurmAssistant3.Areas.Triggers.Data;
using AldursLab.WurmAssistant3.Areas.Triggers.Data.Model;
using AldursLab.WurmAssistant3.Areas.Triggers.Factories;
using AldursLab.WurmAssistant3.Areas.Triggers.ImportExport;
using AldursLab.WurmAssistant3.Areas.Triggers.TriggersManager;
using AldursLab.WurmAssistant3.Properties;
using Caliburn.Micro;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Ninject;

namespace AldursLab.WurmAssistant3.Areas.Triggers
{
    [KernelBind]
    public class TriggerManager : IDisposable
    {
        ActiveTriggers activeTriggers;

        UcPlayerTriggersController controlUi;

        public string CharacterName { get; }

        FormTriggersConfig triggersConfigUi;

        readonly IWurmApi wurmApi;

        readonly ISoundManager soundManager;

        readonly ILogger logger;

        readonly ITrayPopups trayPopups;
        readonly IActiveTriggersFactory activeTriggersFactory;
        readonly IWindowManager windowManager;
        readonly IExporterFactory exporterFactory;
        readonly IImporterFactory importerFactory;

        // previously processed line
        string lastLineContent;

        readonly CharacterTriggersConfig triggersConfig;

        public TriggerManager(
            [NotNull] string characterName,
            [NotNull] IWurmApi wurmApi,
            [NotNull] ISoundManager soundManager, 
            [NotNull] ILogger logger,
            [NotNull] ITrayPopups trayPopups, 
            [NotNull] IActiveTriggersFactory activeTriggersFactory,
            [NotNull] IWindowManager windowManager,
            [NotNull] TriggersDataContext triggersDataContext,
            [NotNull] IExporterFactory exporterFactory,
            [NotNull] IImporterFactory importerFactory)
        {
            if (characterName == null) throw new ArgumentNullException(nameof(characterName));
            if (wurmApi == null) throw new ArgumentNullException(nameof(wurmApi));
            if (soundManager == null) throw new ArgumentNullException(nameof(soundManager));
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            if (trayPopups == null) throw new ArgumentNullException(nameof(trayPopups));
            if (activeTriggersFactory == null) throw new ArgumentNullException(nameof(activeTriggersFactory));
            if (windowManager == null) throw new ArgumentNullException(nameof(windowManager));
            if (triggersDataContext == null) throw new ArgumentNullException(nameof(triggersDataContext));
            if (exporterFactory == null) throw new ArgumentNullException(nameof(exporterFactory));
            if (importerFactory == null) throw new ArgumentNullException(nameof(importerFactory));

            this.CharacterName = characterName;
            this.wurmApi = wurmApi;
            this.soundManager = soundManager;
            this.logger = logger;
            this.trayPopups = trayPopups;
            this.activeTriggersFactory = activeTriggersFactory;
            this.windowManager = windowManager;
            this.exporterFactory = exporterFactory;
            this.importerFactory = importerFactory;
            this.triggersConfig = triggersDataContext.CharacterTriggersConfigs.GetOrCreate(characterName);

            activeTriggers = activeTriggersFactory.CreateActiveTriggers(CharacterName);
            activeTriggers.MutedEvaluator = GetMutedEvaluator();

            //create control for Module UI
            controlUi = new UcPlayerTriggersController();

            //create this notifier UI
            triggersConfigUi = new FormTriggersConfig(this, soundManager, windowManager, exporterFactory, importerFactory);

            UpdateMutedState();
            controlUi.label1.Text = CharacterName;
            controlUi.buttonMute.Click += ToggleMute;
            controlUi.buttonConfigure.Click += Configure;
            controlUi.buttonRemove.Click += StopAndRemove;

            try
            {
                wurmApi.LogsMonitor.Subscribe(this.CharacterName, LogType.AllLogs, OnNewLogEvents);
            }
            catch (DataNotFoundException exception)
            {
                logger.Warn(exception, $"Unable to subscribe LogsMonitor events for character {this.CharacterName}.");
            }
        }

        public TriggersFeature TriggersFeature { get; set; }

        private bool IsMuted()
        {
            return triggersConfig.Muted;
        }

        public Func<bool> GetMutedEvaluator()
        {
            return IsMuted;
        }

        public IEnumerable<ITrigger> Triggers => activeTriggers.All;

        public bool Muted
        {
            get { return triggersConfig.Muted; }
            set { triggersConfig.Muted = value; }
        }

        public byte[] TriggerListState
        {
            get { return triggersConfig.TriggerListState; }
            set { triggersConfig.TriggerListState = value; }
        }

        public void RemoveTrigger(ITrigger trigger)
        {
            activeTriggers.RemoveTrigger(trigger);
        }

        public ITrigger CreateTrigger(TriggerKind kind)
        {
            return activeTriggers.CreateNewTrigger(kind);
        }

        public ITrigger CreateTriggerFromEntity(TriggerEntity triggerEntity)
        {
            return activeTriggers.CreateNewTriggerFromEntity(triggerEntity);
        }

        public UcPlayerTriggersController GetUiHandle()
        {
            return controlUi;
        }

        private void ToggleMute(object sender, EventArgs e)
        {
            triggersConfig.Muted = !triggersConfig.Muted;
            UpdateMutedState();
            triggersConfigUi.UpdateMutedState();
        }

        public void UpdateMutedState()
        {
            controlUi.buttonMute.BackgroundImage = triggersConfig.Muted
                ? Resources.SoundDisabledSmall
                : Resources.SoundEnabledSmall;
        }

        public void Update()
        {
            var dtNow = DateTime.Now;
            foreach (var trigger in Triggers.ToArray())
            {
                trigger.FixedUpdate(dtNow);
            }
        }

        private void Configure(object sender, EventArgs e)
        {
            ToggleUi();
        }

        public void StopAndRemove(object sender, EventArgs e)
        {
            wurmApi.LogsMonitor.Unsubscribe(this.CharacterName, OnNewLogEvents);
            TriggersFeature.RemoveManager(this);

            activeTriggers.DisposeAll();

            controlUi.Dispose();
            triggersConfigUi.Close();
        }

        private void OnNewLogEvents(object sender, LogsMonitorEventArgs e)
        {
            HandleNewLogEvents(e.WurmLogEntries, e.LogType);
        }

        private void ToggleUi()
        {
            triggersConfigUi.ShowAndBringToFront();
        }

        private void HandleNewLogEvents(IEnumerable<LogEntry> newLogEvents, LogType logType)
        {
            lastLineContent = "";
            var dtNow = DateTime.Now;
            foreach (var logMessage in newLogEvents)
            {
                if (lastLineContent != logMessage.Content)
                {
                    foreach (var trigger in Triggers.ToArray())
                    {
                        if (trigger.CheckLogType(logType))
                        {
                            trigger.Update(logMessage, dtNow);
                        }
                    }
                    lastLineContent = logMessage.Content;
                }
            }
        }

        public void Dispose()
        {
            wurmApi.LogsMonitor.Unsubscribe(this.CharacterName, OnNewLogEvents);

            controlUi.Dispose();
            triggersConfigUi.Close();
        }

        public ITrigger FindTriggerById(Guid triggerId)
        {
            return Triggers.FirstOrDefault(trigger => trigger.TriggerId == triggerId);
        }

        public ITrigger FindTriggerByName(string name)
        {
            return Triggers.FirstOrDefault(trigger => string.Equals(trigger.Name, name, StringComparison.CurrentCultureIgnoreCase));
        }
    }
}
