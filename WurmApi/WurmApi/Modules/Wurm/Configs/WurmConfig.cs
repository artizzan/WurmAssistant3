using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using AldursLab.Essentials.Extensions.DotNet;
using AldurSoft.WurmApi.Infrastructure;
using AldurSoft.WurmApi.JobRunning;
using AldurSoft.WurmApi.Modules.Events;
using AldurSoft.WurmApi.Modules.Events.Public;
using JetBrains.Annotations;

namespace AldurSoft.WurmApi.Modules.Wurm.Configs
{
    /// <summary>
    /// If loading config values fails, some or all properties will be null / default.
    /// </summary>
    public class WurmConfig : IDisposable, IWurmConfig
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
            [NotNull] TaskManager taskManager, ILogger logger)
        {
            if (gameSettingsFullPath == null) throw new ArgumentNullException("gameSettingsFullPath");
            if (taskManager == null) throw new ArgumentNullException("taskManager");
            this.gameSettingsFileInfo = new FileInfo(gameSettingsFullPath);
            if (gameSettingsFileInfo.Directory == null)
            {
                throw new WurmApiException("gameSettingsFileInfo.Directory is null, provided file raw path: "
                                           + gameSettingsFullPath);
            }
            Name = gameSettingsFileInfo.Directory.Name;

            this.taskManager = taskManager;

            onConfigChanged = publicEventMarshaller.Create(() => ConfigChanged.SafeInvoke(this),
                WurmApiTuningParams.PublicEventMarshallerDelay);

            this.configReader = new ConfigReader(this);

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

        public string FullConfigFilePath
        {
            get { return this.gameSettingsFileInfo.FullName; }
        }

        public string ConfigDirectoryFullPath
        {
            get
            {
                return this.gameSettingsFileInfo.DirectoryName;
            }
        }

        public void Dispose()
        {
            taskManager.Remove(taskHandle);
            onConfigChanged.Detach();
            configFileWatcher.EnableRaisingEvents = false;
            configFileWatcher.Dispose();
        }

        #region IWurmConfig

        private LogsLocation customTimerSource;
        private LogsLocation execSource;
        private LogsLocation keyBindSource;
        private LogsLocation autoRunSource;
        private LogSaveMode ircLoggingType;
        private LogSaveMode otherLoggingType;
        private LogSaveMode eventLoggingType;
        private SkillGainRate skillGainRate;
        private bool? noSkillMessageOnAlignmentChange;
        private bool? noSkillMessageOnFavorChange;
        private bool? saveSkillsOnQuit;
        private bool? timestampMessages;

        public event EventHandler<EventArgs> ConfigChanged;

        public string Name { get; private set; }

        public LogsLocation CustomTimerSource
        {
            get
            {
                return customTimerSource;
            }
        }

        public LogsLocation ExecSource
        {
            get
            {
                return execSource;
            }
        }

        public LogsLocation KeyBindSource
        {
            get
            {
                return keyBindSource;
            }
        }

        public LogsLocation AutoRunSource
        {
            get
            {
                return autoRunSource;
            }
        }

        public LogSaveMode IrcLoggingType
        {
            get
            {
                return ircLoggingType;
            }
        }

        public LogSaveMode OtherLoggingType
        {
            get
            {
                return otherLoggingType;
            }
        }

        public LogSaveMode EventLoggingType
        {
            get
            {
                return eventLoggingType;
            }
        }

        public SkillGainRate SkillGainRate
        {
            get
            {
                return skillGainRate;
            }
        }

        public bool? NoSkillMessageOnAlignmentChange
        {
            get
            {
                return noSkillMessageOnAlignmentChange;
            }
        }

        public bool? NoSkillMessageOnFavorChange
        {
            get
            {
                return noSkillMessageOnFavorChange;
            }
        }

        public bool? SaveSkillsOnQuit
        {
            get
            {
                return saveSkillsOnQuit;
            }
        }

        public bool? TimestampMessages
        {
            get
            {
                return timestampMessages;
            }
        }

        void Refresh()
        {
            lock (locker)
            {
                var result = this.configReader.ReadValues();
                this.customTimerSource = result.CustomTimerSource;
                this.execSource = result.ExecSource;
                this.keyBindSource = result.KeyBindSource;
                this.autoRunSource = result.AutoRunSource;
                this.ircLoggingType = result.IrcLoggingType;
                this.otherLoggingType = result.OtherLoggingType;
                this.eventLoggingType = result.EventLoggingType;
                this.skillGainRate = result.SkillGainRate;
                this.noSkillMessageOnAlignmentChange = result.NoSkillMessageOnAlignmentChange;
                this.noSkillMessageOnFavorChange = result.NoSkillMessageOnFavorChange;
                this.saveSkillsOnQuit = result.SaveSkillsOnQuit;
                this.timestampMessages = result.TimestampMessages;

                this.HasBeenRead = true;
            }
            onConfigChanged.Trigger();
        }

        public bool HasBeenRead { get; private set; }

        #endregion
    }
}