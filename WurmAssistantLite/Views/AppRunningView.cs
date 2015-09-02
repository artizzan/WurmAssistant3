using System;
using System.Drawing;
using System.Linq;
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

            //todo: dedicated view
            var moduleManager = appRunningViewModel.ModuleManagerViewModel;
            foreach (var c in moduleManager.ModuleControlViewModels)
            {
                var control = c;
                Button btn = new Button();
                btn.Size = new Size(80, 80);
                btn.Click += (sender, args) => control.Open();
                btn.Text = c.Name;
                moduleButtonBar.Controls.Add(btn);
            }
        }

        private void btnResetConfig_Click(object sender, EventArgs e)
        {
            wurmAssistantLiteSettings.SetupRequired = true;
            appRunningViewModel.RequestRestart();
        }
    }
}
