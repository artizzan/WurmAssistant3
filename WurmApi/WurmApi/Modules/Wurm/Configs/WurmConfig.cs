using System;
using System.IO;
using System.Threading;
using AldurSoft.Core;
using AldurSoft.WurmApi.Infrastructure;
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
        // setters are maintained only to support testing, until tests are properly refactored todo

        readonly FileInfo gameSettingsFileInfo;
        readonly ILogger logger;
        readonly IEventMarshaller eventMarshaller;
        readonly ConfigReader configReader;
        readonly ConfigWriter configWriter;

        readonly FileSystemWatcher configFileWatcher;
        readonly object locker = new object();

        internal WurmConfig(string gameSettingsFullPath, [NotNull] ILogger logger,
            [NotNull] IEventMarshaller eventMarshaller)
        {
            if (gameSettingsFullPath == null) throw new ArgumentNullException("gameSettingsFullPath");
            if (logger == null) throw new ArgumentNullException("logger");
            if (eventMarshaller == null) throw new ArgumentNullException("eventMarshaller");
            this.gameSettingsFileInfo = new FileInfo(gameSettingsFullPath);
            if (gameSettingsFileInfo.Directory == null)
            {
                throw new WurmApiException("gameSettingsFileInfo.Directory is null, provided file raw path: "
                                           + gameSettingsFullPath);
            }
            Name = gameSettingsFileInfo.Directory.Name;

            this.logger = logger;
            this.eventMarshaller = eventMarshaller;
            this.configReader = new ConfigReader(this);
            this.configWriter = new ConfigWriter(this);

            configFileWatcher = new FileSystemWatcher(gameSettingsFileInfo.Directory.FullName)
            {
                Filter = gameSettingsFileInfo.Name
            };
            configFileWatcher.Changed += ConfigFileWatcherOnChanged;
            configFileWatcher.Created += ConfigFileWatcherOnChanged;
            configFileWatcher.Deleted += ConfigFileWatcherOnChanged;
            configFileWatcher.Renamed += ConfigFileWatcherOnChanged;
            configFileWatcher.EnableRaisingEvents = true;
        }

        void ConfigFileWatcherOnChanged(object sender, FileSystemEventArgs fileSystemEventArgs)
        {
            this.isCacheValid = 0;
        }

        private volatile int isCacheValid = 0;
        private bool IsCacheValid { get { return isCacheValid == 0; } }

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

        public event EventHandler ConfigChanged;

        public string Name { get; private set; }

        protected virtual void OnConfigChanged()
        {
            try
            {
                EventHandler handler = ConfigChanged;
                if (handler != null)
                {
                    eventMarshaller.Marshal(() => handler(this, EventArgs.Empty));
                }
            }
            catch (Exception exception)
            {
                logger.Log(LogLevel.Error, "OnConfigChanged delegate threw exception", this, exception);
            }
        }

        public LogsLocation CustomTimerSource
        {
            get
            {
                RefreshValues();
                return customTimerSource;
            }
        }

        public LogsLocation ExecSource
        {
            get
            {
                RefreshValues();
                return execSource;
            }
        }

        public LogsLocation KeyBindSource
        {
            get
            {
                RefreshValues();
                return keyBindSource;
            }
        }

        public LogsLocation AutoRunSource
        {
            get
            {
                RefreshValues();
                return autoRunSource;
            }
        }

        public LogSaveMode IrcLoggingType
        {
            get
            {
                RefreshValues();
                return ircLoggingType;
            }
            set
            {
                this.configWriter.SetIrcLoggingMode(value);
            }
        }

        public LogSaveMode OtherLoggingType
        {
            get
            {
                RefreshValues();
                return otherLoggingType;
            }
            set
            {
                this.configWriter.SetOtherLoggingMode(value);
            }
        }

        public LogSaveMode EventLoggingType
        {
            get
            {
                RefreshValues();
                return eventLoggingType;
            }
            set
            {
                this.configWriter.SetEventLoggingMode(value);
            }
        }

        public SkillGainRate SkillGainRate
        {
            get
            {
                RefreshValues();
                return skillGainRate;
            }
            set
            {
                this.configWriter.SetSkillGainRate(value);
            }
        }

        public bool? NoSkillMessageOnAlignmentChange
        {
            get
            {
                RefreshValues();
                return noSkillMessageOnAlignmentChange;
            }
            set
            {
                this.configWriter.SetNoSkillMessageOnAlignmentChange(value);
            }
        }

        public bool? NoSkillMessageOnFavorChange
        {
            get
            {
                RefreshValues();
                return noSkillMessageOnFavorChange;
            }
            set
            {
                this.configWriter.SetNoSkillMessageOnFavorChange(value);
            }
        }

        public bool? SaveSkillsOnQuit
        {
            get
            {
                RefreshValues();
                return saveSkillsOnQuit;
            }
            set
            {
                this.configWriter.SetSaveSkillsOnQuit(value);
            }
        }

        public bool? TimestampMessages
        {
            get
            {
                RefreshValues();
                return timestampMessages;
            }
            set
            {
                this.configWriter.SetTimestampMessages(value);
            }
        }

        private void RefreshValues()
        {
            if (!IsCacheValid)
            {
                bool changed = false;
                lock (locker)
                {
                    if (Interlocked.CompareExchange(ref isCacheValid, 1, 0) == 0)
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
                        changed = true;
                    }
                }
                if (changed)
                {
                    OnConfigChanged();
                }
            }
        }

        #endregion
    }
}