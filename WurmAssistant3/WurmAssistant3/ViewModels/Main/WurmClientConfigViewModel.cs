using System;
using AldursLab.Deprec.Core;
using AldursLab.Deprec.Core.AppFramework.Wpf.ViewModels;
using AldursLab.WurmAssistant3.Systems;

namespace AldursLab.WurmAssistant3.ViewModels.Main
{
    public class WurmClientConfigViewModel : ViewModelBase
    {
        private const string IgnoreAndShutdownDesc1 =
            "I will run Wurm Assistant later. Don't do anything for now.\r\n(Exit Wurm Assistant)";
        private const string IgnoreAndShutdownDesc2 = "Cancel";

        private readonly IWurmAssistantSettings wurmAssistantSettings;
        private readonly IWurmApiConfigurator wurmApiConfigurator;
        private LogSaveMode _logSaveMode;
        private SkillGainRate _skillGainRate;
        private bool _doNotShowThisWindow;
        private string _issues;
        private bool _regularDisplayMode;

        public WurmClientConfigViewModel(
            [NotNull] IWurmAssistantSettings wurmAssistantSettings,
            [NotNull] IWurmApiConfigurator wurmApiConfigurator)
        {
            if (wurmAssistantSettings == null) throw new ArgumentNullException("wurmAssistantSettings");
            if (wurmApiConfigurator == null) throw new ArgumentNullException("wurmApiConfigurator");
            this.wurmAssistantSettings = wurmAssistantSettings;
            this.wurmApiConfigurator = wurmApiConfigurator;
            LogSaveMode = wurmAssistantSettings.Entity.WurmClientConfig.LogSaveMode;
            SkillGainRate = wurmAssistantSettings.Entity.WurmClientConfig.SkillGainRate;
            DoNotShowThisWindow = wurmAssistantSettings.Entity.WurmClientConfig.DoNotAskToSyncWurmClients;
            var verifyResult = wurmApiConfigurator.VerifyGameClientConfig();
            Issues = verifyResult.ToString();
        }

        public string DoNothingAndShutdownBtnDesc
        {
            get
            {
                if (!RegularDisplayMode) return IgnoreAndShutdownDesc1;
                else return IgnoreAndShutdownDesc2;
            }
        }

        /// <summary>
        /// False for bootstrapper display (before WA run). True for regular display (options menu)
        /// </summary>
        public bool RegularDisplayMode
        {
            get { return _regularDisplayMode; }
            set
            {
                if (value.Equals(_regularDisplayMode)) return;
                _regularDisplayMode = value;
                NotifyOfPropertyChange(() => RegularDisplayMode);
                NotifyOfPropertyChange(() => DoNothingAndShutdownBtnDesc);
            }
        }

        public string Issues
        {
            get { return _issues; }
            set
            {
                if (value == _issues) return;
                _issues = value;
                NotifyOfPropertyChange(() => Issues);
            }
        }

        public LogSaveMode LogSaveMode
        {
            get { return _logSaveMode; }
            set
            {
                if (value == _logSaveMode) return;
                _logSaveMode = value;
                NotifyOfPropertyChange(() => LogSaveMode);
                NotifyOfPropertyChange(() => MonthlyLogging);
                NotifyOfPropertyChange(() => DailyLogging);
                switch (value)
                {
                    case LogSaveMode.Daily:
                        DailyLogging = true;
                        break;
                    case LogSaveMode.Monthly:
                        MonthlyLogging = true;
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        public SkillGainRate SkillGainRate
        {
            get { return _skillGainRate; }
            set
            {
                if (value == _skillGainRate) return;
                _skillGainRate = value;
                NotifyOfPropertyChange(() => SkillGainRate);
                NotifyOfPropertyChange(() => AlwaysSkillGainMode);
                NotifyOfPropertyChange(() => Per001SkillGainMode);
                NotifyOfPropertyChange(() => Per01SkillGainMode);
                switch (value)
                {
                    case SkillGainRate.Always:
                        AlwaysSkillGainMode = true;
                        break;
                    case SkillGainRate.Per0D001:
                        Per001SkillGainMode = true;
                        break;
                    case SkillGainRate.Per0D01:
                        Per01SkillGainMode = true;
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        public bool DoNotShowThisWindow
        {
            get { return _doNotShowThisWindow; }
            set
            {
                if (value.Equals(_doNotShowThisWindow)) return;
                _doNotShowThisWindow = value;
                NotifyOfPropertyChange(() => DoNotShowThisWindow);
            }
        }

        public bool DailyLogging
        {
            get { return LogSaveMode == LogSaveMode.Daily; }
            set
            {
                if (LogSaveMode == LogSaveMode.Daily) return;
                LogSaveMode = LogSaveMode.Daily;
                NotifyOfPropertyChange(() => DailyLogging);
                NotifyOfPropertyChange(() => LogSaveMode);
            }
        }

        public bool MonthlyLogging
        {
            get { return LogSaveMode == LogSaveMode.Monthly; }
            set
            {
                if (LogSaveMode == LogSaveMode.Monthly) return;
                LogSaveMode = LogSaveMode.Monthly;
                NotifyOfPropertyChange(() => MonthlyLogging);
                NotifyOfPropertyChange(() => LogSaveMode);
            }
        }

        public bool AlwaysSkillGainMode
        {
            get { return SkillGainRate == SkillGainRate.Always; }
            set
            {
                if (SkillGainRate == SkillGainRate.Always) return;
                SkillGainRate = SkillGainRate.Always;
                NotifyOfPropertyChange(() => AlwaysSkillGainMode);
                NotifyOfPropertyChange(() => SkillGainRate);
            }
        }

        public bool Per001SkillGainMode
        {
            get { return SkillGainRate == SkillGainRate.Per0D001; }
            set
            {
                if (SkillGainRate == SkillGainRate.Per0D001)
                    return;
                SkillGainRate = SkillGainRate.Per0D001;
                NotifyOfPropertyChange(() => Per001SkillGainMode);
                NotifyOfPropertyChange(() => SkillGainRate);
            }
        }

        public bool Per01SkillGainMode
        {
            get { return SkillGainRate == SkillGainRate.Per0D01; }
            set
            {
                if (SkillGainRate == SkillGainRate.Per0D01)
                    return;
                SkillGainRate = SkillGainRate.Per0D01;
                NotifyOfPropertyChange(() => Per01SkillGainMode);
                NotifyOfPropertyChange(() => SkillGainRate);
            }
        }

        public void ApplyAndRunWa()
        {
            wurmApiConfigurator.AppendTimeQueryToAutoruns();
            wurmApiConfigurator.AppendUptimeQueryToAutoruns();
            wurmApiConfigurator.SynchronizeWurmGameClientSettings();
            SaveSettings();
            Result = ResultValue.SyncConfigsAndContinue;
        }

        public void DoNothingAndRunWa()
        {
            SaveSettings();
            Result = ResultValue.IgnoreAndContinue;
        }

        public void DoNothingAndShutdown()
        {
            SaveSettings();
            Result = ResultValue.IgnoreAndShutdown;
        }

        private void SaveSettings()
        {
            wurmAssistantSettings.Entity.WurmClientConfig.LogSaveMode = LogSaveMode;
            wurmAssistantSettings.Entity.WurmClientConfig.SkillGainRate = SkillGainRate;
            wurmAssistantSettings.Entity.WurmClientConfig.DoNotAskToSyncWurmClients = DoNotShowThisWindow;
            wurmAssistantSettings.Save();
        }

        public ResultValue Result { get; private set; }
            
        public enum ResultValue
        {
            IgnoreAndShutdown,
            IgnoreAndContinue,
            SyncConfigsAndContinue
        }
    }
}
