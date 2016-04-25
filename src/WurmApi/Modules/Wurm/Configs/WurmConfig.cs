using System;
using System.IO;
using AldursLab.WurmApi.Extensions.DotNet;
using AldursLab.WurmApi.JobRunning;
using AldursLab.WurmApi.Modules.Events.Public;
using JetBrains.Annotations;

namespace AldursLab.WurmApi.Modules.Wurm.Configs
{
    /// <summary>
    /// If loading config values fails, some or all properties will be null / default.
    /// </summary>
    class WurmConfig : IDisposable, IWurmConfig
    {
        // note: editing config through IWurmApi is no longer allowed,
        // because there is no good way to ensure, that Wurm Game Client isn't accessing it at the moment

        readonly FileInfo gameSettingsFileInfo;
        readonly TaskManager taskManager;

        readonly ConfigReader configReader;

        readonly FileSystemWatcher configFileWatcher;
        readonly object locker = new object();

        readonly PublicEvent onConfigChanged;

        readonly TaskHandle taskHandle;

        internal WurmConfig(string gameSettingsFullPath, [NotNull] IPublicEventInvoker publicEventMarshaller,
            [NotNull] TaskManager taskManager, IWurmApiLogger logger)
        {
            if (gameSettingsFullPath == null) throw new ArgumentNullException(nameof(gameSettingsFullPath));
            if (taskManager == null) throw new ArgumentNullException(nameof(taskManager));
            gameSettingsFileInfo = new FileInfo(gameSettingsFullPath);
            if (gameSettingsFileInfo.Directory == null)
            {
                throw new WurmApiException("gameSettingsFileInfo.Directory is null, provided file raw path: "
                                           + gameSettingsFullPath);
            }
            Name = gameSettingsFileInfo.Directory.Name;

            this.taskManager = taskManager;

            onConfigChanged = publicEventMarshaller.Create(() => ConfigChanged.SafeInvoke(this, EventArgs.Empty),
                WurmApiTuningParams.PublicEventMarshallerDelay);

            configReader = new ConfigReader(this);

            try
            {
                Refresh();
            }
            catch (Exception exception)
            {
                logger.Log(LogLevel.Error, "Error at initial config update: " + Name, this, exception);
            }

            configFileWatcher = new FileSystemWatcher(gameSettingsFileInfo.Directory.FullName)
            {
                Filter = gameSettingsFileInfo.Name,
                NotifyFilter = NotifyFilters.Size | NotifyFilters.LastWrite
            };
            configFileWatcher.Changed += ConfigFileWatcherOnChanged;
            configFileWatcher.Created += ConfigFileWatcherOnChanged;
            configFileWatcher.Deleted += ConfigFileWatcherOnChanged;
            configFileWatcher.Renamed += ConfigFileWatcherOnChanged;
            configFileWatcher.EnableRaisingEvents = true;

            taskHandle = new TaskHandle(Refresh, "WurmConfig update: " + Name);
            taskManager.Add(taskHandle);

            taskHandle.Trigger();
        }

        void ConfigFileWatcherOnChanged(object sender, FileSystemEventArgs fileSystemEventArgs)
        {
            //Trace.WriteLine(DateTime.Now.ToString("O"));
            taskHandle.Trigger();
        }

        public string FullConfigFilePath => gameSettingsFileInfo.FullName;

        public string ConfigDirectoryFullPath => gameSettingsFileInfo.DirectoryName;

        public void Dispose()
        {
            taskManager.Remove(taskHandle);
            onConfigChanged.Detach();
            configFileWatcher.EnableRaisingEvents = false;
            configFileWatcher.Dispose();
        }

        #region IWurmConfig

        LogsLocation customTimerSource;
        LogsLocation execSource;
        LogsLocation keyBindSource;
        LogsLocation autoRunSource;
        LogSaveMode ircLoggingType;
        LogSaveMode otherLoggingType;
        LogSaveMode eventLoggingType;
        SkillGainRate skillGainRate;
        bool? noSkillMessageOnAlignmentChange;
        bool? noSkillMessageOnFavorChange;
        bool? saveSkillsOnQuit;
        bool? timestampMessages;
        bool? saveSkillDumpsOnQuit;

        public event EventHandler<EventArgs> ConfigChanged;

        public string Name { get; private set; }

        public LogsLocation CustomTimerSource => customTimerSource;

        public LogsLocation ExecSource => execSource;

        public LogsLocation KeyBindSource => keyBindSource;

        public LogsLocation AutoRunSource => autoRunSource;

        public LogSaveMode IrcLoggingType => ircLoggingType;

        public LogSaveMode OtherLoggingType => otherLoggingType;

        public LogSaveMode EventLoggingType => eventLoggingType;

        public SkillGainRate SkillGainRate => skillGainRate;

        public bool? NoSkillMessageOnAlignmentChange => noSkillMessageOnAlignmentChange;

        public bool? NoSkillMessageOnFavorChange => noSkillMessageOnFavorChange;

        public bool? SaveSkillsOnQuit => saveSkillsOnQuit;

        public bool? TimestampMessages => timestampMessages;

        void Refresh()
        {
            lock (locker)
            {
                var result = configReader.ReadValues();
                customTimerSource = result.CustomTimerSource;
                execSource = result.ExecSource;
                keyBindSource = result.KeyBindSource;
                autoRunSource = result.AutoRunSource;
                ircLoggingType = result.IrcLoggingType;
                otherLoggingType = result.OtherLoggingType;
                eventLoggingType = result.EventLoggingType;
                skillGainRate = result.SkillGainRate;
                noSkillMessageOnAlignmentChange = result.NoSkillMessageOnAlignmentChange;
                noSkillMessageOnFavorChange = result.NoSkillMessageOnFavorChange;
                saveSkillsOnQuit = result.SaveSkillsOnQuit;
                timestampMessages = result.TimestampMessages;

                HasBeenRead = true;
            }
            onConfigChanged.Trigger();
        }

        public bool HasBeenRead { get; private set; }

        #endregion
    }
}