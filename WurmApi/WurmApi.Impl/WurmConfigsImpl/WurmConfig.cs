using System;
using System.IO;

using AldurSoft.Core;

namespace AldurSoft.WurmApi.Impl.WurmConfigsImpl
{
    /// <summary>
    /// If loading config values fails, some or all properties will be null / default.
    /// </summary>
    public class WurmConfig : IDisposable, IWurmConfig, IRequireRefresh
    {
        private readonly FileInfo gameSettingsFileInfo;
        private readonly IWurmGameClients wurmGameClientsImpl;
        private readonly IThreadGuard threadGuard;
        private readonly ConfigReader configReader;
        private readonly ConfigWriter configWriter;

        private readonly FileSystemChangeMonitor fileSystemChangeMonitor;

        internal WurmConfig(string gameSettingsFullPath, IWurmGameClients wurmGameClientsImpl,
            IThreadGuard threadGuard)
        {
            if (gameSettingsFullPath == null) throw new ArgumentNullException("gameSettingsFullPath");
            if (wurmGameClientsImpl == null) throw new ArgumentNullException("wurmGameClientsImpl");
            if (threadGuard == null) throw new ArgumentNullException("threadGuard");
            this.gameSettingsFileInfo = new FileInfo(gameSettingsFullPath);

            if (gameSettingsFileInfo.Directory == null)
            {
                Name = string.Empty;
            }
            else
            {
                Name = gameSettingsFileInfo.Directory.Name;
            }

            this.wurmGameClientsImpl = wurmGameClientsImpl;
            this.threadGuard = threadGuard;
            this.configReader = new ConfigReader(this);
            this.configWriter = new ConfigWriter(this);

            this.fileSystemChangeMonitor = new FileSystemChangeMonitor
            {
                FullPath = gameSettingsFileInfo.DirectoryName,
                Filter = gameSettingsFileInfo.Name
            };
            fileSystemChangeMonitor.EnableRaisingEvents = true;
        }

        private bool isCacheValid = false;

        public string FullConfigFilePath
        {
            get { return this.gameSettingsFileInfo.FullName; }
        }

        public string ConfigDirectoryFullPath
        {
            get
            {
                threadGuard.ValidateCurrentThread();
                return this.gameSettingsFileInfo.DirectoryName;
            }
        }

        public void Dispose()
        {
            this.fileSystemChangeMonitor.Dispose();
        }

        #region IWurmConfig

        private LogsLocation _customTimerSource;
        private LogsLocation _execSource;
        private LogsLocation _keyBindSource;
        private LogsLocation _autoRunSource;
        private LogSaveMode _ircLoggingType;
        private LogSaveMode _otherLoggingType;
        private LogSaveMode _eventLoggingType;
        private SkillGainRate _skillGainRate;
        private bool? _noSkillMessageOnAlignmentChange;
        private bool? _noSkillMessageOnFavorChange;
        private bool? _saveSkillsOnQuit;
        private bool? _timestampMessages;

        public event EventHandler ConfigChanged;

        public string Name { get; private set; }

        protected virtual void OnConfigChanged()
        {
            EventHandler handler = ConfigChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public LogsLocation CustomTimerSource
        {
            get
            {
                if (!this.isCacheValid)
                {
                    threadGuard.ValidateCurrentThread();
                    this.LoadValues();
                }
                return this._customTimerSource;
            }
        }

        public LogsLocation ExecSource
        {
            get
            {
                if (!this.isCacheValid)
                {
                    threadGuard.ValidateCurrentThread();
                    this.LoadValues();
                }
                return this._execSource;
            }
        }

        public LogsLocation KeyBindSource
        {
            get
            {
                if (!this.isCacheValid)
                {
                    threadGuard.ValidateCurrentThread();
                    this.LoadValues();
                }
                return this._keyBindSource;
            }
        }

        public LogsLocation AutoRunSource
        {
            get
            {
                if (!this.isCacheValid)
                {
                    threadGuard.ValidateCurrentThread();
                    this.LoadValues();
                }
                return this._autoRunSource;
            }
        }

        public LogSaveMode IrcLoggingType
        {
            get
            {
                if (!this.isCacheValid)
                {
                    threadGuard.ValidateCurrentThread();
                    this.LoadValues();
                }
                return this._ircLoggingType;
            }
            set
            {
                threadGuard.ValidateCurrentThread();
                this.AssertCanChangeConfig();
                this.configWriter.SetIrcLoggingMode(value);
                this.isCacheValid = false;
            }
        }

        public LogSaveMode OtherLoggingType
        {
            get
            {
                if (!this.isCacheValid)
                {
                    threadGuard.ValidateCurrentThread();
                    this.LoadValues();
                }
                return this._otherLoggingType;
            }
            set
            {
                threadGuard.ValidateCurrentThread();
                this.AssertCanChangeConfig();
                this.configWriter.SetOtherLoggingMode(value);
                this.isCacheValid = false;
            }
        }

        public LogSaveMode EventLoggingType
        {
            get
            {
                if (!this.isCacheValid)
                {
                    threadGuard.ValidateCurrentThread();
                    this.LoadValues();
                }
                return this._eventLoggingType;
            }
            set
            {
                threadGuard.ValidateCurrentThread();
                this.AssertCanChangeConfig();
                this.configWriter.SetEventLoggingMode(value);
                this.isCacheValid = false;
            }
        }

        public SkillGainRate SkillGainRate
        {
            get
            {
                if (!this.isCacheValid)
                {
                    threadGuard.ValidateCurrentThread();
                    this.LoadValues();
                }
                return this._skillGainRate;
            }
            set
            {
                threadGuard.ValidateCurrentThread();
                this.AssertCanChangeConfig();
                this.configWriter.SetSkillGainRate(value);
                this.isCacheValid = false;
            }
        }

        public bool? NoSkillMessageOnAlignmentChange
        {
            get
            {
                if (!this.isCacheValid)
                {
                    threadGuard.ValidateCurrentThread();
                    this.LoadValues();
                }
                return this._noSkillMessageOnAlignmentChange;
            }
            set
            {
                threadGuard.ValidateCurrentThread();
                this.AssertCanChangeConfig();
                this.configWriter.SetNoSkillMessageOnAlignmentChange(value);
                this.isCacheValid = false;
            }
        }

        public bool? NoSkillMessageOnFavorChange
        {
            get
            {
                if (!this.isCacheValid)
                {
                    threadGuard.ValidateCurrentThread();
                    this.LoadValues();
                }
                return this._noSkillMessageOnFavorChange;
            }
            set
            {
                threadGuard.ValidateCurrentThread();
                this.AssertCanChangeConfig();
                this.configWriter.SetNoSkillMessageOnFavorChange(value);
                this.isCacheValid = false;
            }
        }

        public bool? SaveSkillsOnQuit
        {
            get
            {
                if (!this.isCacheValid)
                {
                    threadGuard.ValidateCurrentThread();
                    this.LoadValues();
                }
                return this._saveSkillsOnQuit;
            }
            set
            {
                threadGuard.ValidateCurrentThread();
                this.AssertCanChangeConfig();
                this.configWriter.SetSaveSkillsOnQuit(value);
                this.isCacheValid = false;
            }
        }

        public bool? TimestampMessages
        {
            get
            {
                if (!this.isCacheValid)
                {
                    threadGuard.ValidateCurrentThread();
                    this.LoadValues();
                }
                return this._timestampMessages;
            }
            set
            {
                threadGuard.ValidateCurrentThread();
                this.AssertCanChangeConfig();
                this.configWriter.SetTimestampMessages(value);
                this.isCacheValid = false;
            }
        }

        private void LoadValues()
        {
            var result = this.configReader.ReadValues();
            this._customTimerSource = result.CustomTimerSource;
            this._execSource = result.ExecSource;
            this._keyBindSource = result.KeyBindSource;
            this._autoRunSource = result.AutoRunSource;
            this._ircLoggingType = result.IrcLoggingType;
            this._otherLoggingType = result.OtherLoggingType;
            this._eventLoggingType = result.EventLoggingType;
            this._skillGainRate = result.SkillGainRate;
            this._noSkillMessageOnAlignmentChange = result.NoSkillMessageOnAlignmentChange;
            this._noSkillMessageOnFavorChange = result.NoSkillMessageOnFavorChange;
            this._saveSkillsOnQuit = result.SaveSkillsOnQuit;
            this._timestampMessages = result.TimestampMessages;
            this.isCacheValid = true;
        }

        private void AssertCanChangeConfig()
        {
            if (!this.CanChangeConfig)
            {
                throw new WurmApiException(
                    "Cannot change config, because at least one Wurm instance is running");
            }
        }

        public bool CanChangeConfig
        {
            get
            {
                threadGuard.ValidateCurrentThread();
                // disabling, because old detection method is broken
                return true; //!this.wurmGameClientsImpl.AnyRunning;
            }
        }

        #endregion

        public void Refresh()
        {
            if (fileSystemChangeMonitor.GetAnyChangeAndReset())
            {
                this.isCacheValid = false;
                OnConfigChanged();
            }
        }
    }
}