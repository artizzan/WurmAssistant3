using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using AldurSoft.WurmApi.Infrastructure;
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
        readonly ILogger logger;
        readonly IPublicEventInvoker publicEventMarshaller;

        readonly ConfigReader configReader;
        readonly ConfigWriter configWriter;

        readonly FileSystemWatcher configFileWatcher;
        readonly object locker = new object();

        private volatile int rebuildPending = 1;

        readonly PublicEvent onConfigChanged;

        internal WurmConfig(string gameSettingsFullPath, [NotNull] ILogger logger,
            [NotNull] IPublicEventInvoker publicEventMarshaller)
        {
            if (gameSettingsFullPath == null) throw new ArgumentNullException("gameSettingsFullPath");
            if (logger == null) throw new ArgumentNullException("logger");
            if (publicEventMarshaller == null) throw new ArgumentNullException("publicEventMarshaller");
            this.gameSettingsFileInfo = new FileInfo(gameSettingsFullPath);
            if (gameSettingsFileInfo.Directory == null)
            {
                throw new WurmApiException("gameSettingsFileInfo.Directory is null, provided file raw path: "
                                           + gameSettingsFullPath);
            }
            Name = gameSettingsFileInfo.Directory.Name;

            this.logger = logger;
            this.publicEventMarshaller = publicEventMarshaller;

            onConfigChanged = publicEventMarshaller.Create(() => ConfigChanged.SafeInvoke(this),
                TimeSpan.FromMilliseconds(500));

            this.configReader = new ConfigReader(this);
            this.configWriter = new ConfigWriter(this);

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
        }

        void ConfigFileWatcherOnChanged(object sender, FileSystemEventArgs fileSystemEventArgs)
        {
            this.rebuildPending = 1;
            onConfigChanged.Trigger();
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
                Refresh();
                return customTimerSource;
            }
        }

        public LogsLocation ExecSource
        {
            get
            {
                Refresh();
                return execSource;
            }
        }

        public LogsLocation KeyBindSource
        {
            get
            {
                Refresh();
                return keyBindSource;
            }
        }

        public LogsLocation AutoRunSource
        {
            get
            {
                Refresh();
                return autoRunSource;
            }
        }

        public LogSaveMode IrcLoggingType
        {
            get
            {
                Refresh();
                return ircLoggingType;
            }
        }

        public LogSaveMode OtherLoggingType
        {
            get
            {
                Refresh();
                return otherLoggingType;
            }
        }

        public LogSaveMode EventLoggingType
        {
            get
            {
                Refresh();
                return eventLoggingType;
            }
        }

        public SkillGainRate SkillGainRate
        {
            get
            {
                Refresh();
                return skillGainRate;
            }
        }

        public bool? NoSkillMessageOnAlignmentChange
        {
            get
            {
                Refresh();
                return noSkillMessageOnAlignmentChange;
            }
        }

        public bool? NoSkillMessageOnFavorChange
        {
            get
            {
                Refresh();
                return noSkillMessageOnFavorChange;
            }
        }

        public bool? SaveSkillsOnQuit
        {
            get
            {
                Refresh();
                return saveSkillsOnQuit;
            }
        }

        public bool? TimestampMessages
        {
            get
            {
                Refresh();
                return timestampMessages;
            }
        }

        private void Refresh()
        {
            if (rebuildPending == 1)
            {
                lock (locker)
                {
                    if (Interlocked.CompareExchange(ref rebuildPending, 0, 1) == 1)
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
                    }
                }
            }
        }

        #endregion
    }
}