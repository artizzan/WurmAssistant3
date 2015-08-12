using System;
using AldursLab.Deprec.Core;
using AldursLab.Deprec.Core.AppFramework.Wpf.ViewModels;
using AldursLab.WurmAssistant3.Systems;

namespace AldursLab.WurmAssistant3.ViewModels.Main
{
    public class WurmClientInstallDirChooserViewModel : ViewModelBase
    {
        private readonly IWurmAssistantSettings wurmAssistantSettings;
        private string _installDirFullPath;
        private bool _canApply;
        private string _errorLabel;
        private string _descriptonTextBlock;

        public WurmClientInstallDirChooserViewModel(
            [NotNull] IWurmAssistantSettings wurmAssistantSettings)
        {
            if (wurmAssistantSettings == null) throw new ArgumentNullException("wurmAssistantSettings");
            this.wurmAssistantSettings = wurmAssistantSettings;
            if (wurmAssistantSettings.Entity.WurmClientConfig.WurmClientInstallDirOverride != null)
            {
                InstallDirFullPath = wurmAssistantSettings.Entity.WurmClientConfig.WurmClientInstallDirOverride;
            }
            ValidateWurmDir();
        }

        public string InstallDirFullPath
        {
            get { return _installDirFullPath; }
            set
            {
                if (value == _installDirFullPath) return;
                _installDirFullPath = value;
                ValidateWurmDir();
                NotifyOfPropertyChange(() => InstallDirFullPath);
            }
        }

        public bool CanApply
        {
            get { return _canApply; }
            private set
            {
                if (value.Equals(_canApply)) return;
                _canApply = value;
                NotifyOfPropertyChange(() => CanApply);
            }
        }

        public string ErrorLabel
        {
            get { return _errorLabel; }
            set
            {
                if (value == _errorLabel) return;
                _errorLabel = value;
                NotifyOfPropertyChange(() => ErrorLabel);
            }
        }

        public string DescriptonTextBlock
        {
            get { return _descriptonTextBlock; }
            set
            {
                if (value == _descriptonTextBlock) return;
                _descriptonTextBlock = value;
                NotifyOfPropertyChange(() => DescriptonTextBlock);
            }
        }

        public bool ValidateWurmDir()
        {
            try
            {
                var idir = new WurmGameClientInstallDirectory(InstallDirFullPath);
                CanApply = true;
                ErrorLabel = string.Empty;
                return true;
            }
            catch (WurmGameClientInstallDirectoryValidationException exception)
            {
                ErrorLabel = exception.Message;
                CanApply = false;
                return false;
            }
        }

        public void DetectPath()
        {
            try
            {
                var idir = new WurmGameClientInstallDirectory();
                InstallDirFullPath = idir.FullPath;
            }
            catch (WurmGameClientInstallDirectoryValidationException exception)
            {
                ErrorLabel = exception.Message;
                CanApply = false;
            }
        }
    }
}
