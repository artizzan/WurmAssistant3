using System;
using System.Windows.Forms;
using AldursLab.WurmAssistant3.Core.Infrastructure;
using AldursLab.WurmAssistant3.Core.ViewModels;
using AldursLab.WurmAssistantLite.Bootstrapping.Persistent;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistantLite.Views
{
    public partial class AppRunningView : UserControl
    {
        readonly AppRunningViewModel appRunningViewModel;
        readonly WurmAssistantLiteSettings wurmAssistantLiteSettings;

        public AppRunningView()
        {
            InitializeComponent();
        }

        public AppRunningView([NotNull] AppRunningViewModel appRunningViewModel, [NotNull] WurmAssistantLiteSettings wurmAssistantLiteSettings)
            : this()
        {
            if (appRunningViewModel == null) throw new ArgumentNullException("appRunningViewModel");
            if (wurmAssistantLiteSettings == null) throw new ArgumentNullException("wurmAssistantLiteSettings");
            this.appRunningViewModel = appRunningViewModel;
            this.wurmAssistantLiteSettings = wurmAssistantLiteSettings;
        }

        private void btnResetConfig_Click(object sender, EventArgs e)
        {
            wurmAssistantLiteSettings.SetupRequired = true;
            appRunningViewModel.RequestRestart();
        }
    }
}
