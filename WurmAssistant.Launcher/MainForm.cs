using System;
using System.IO;
using System.Windows.Forms;
using AldursLab.Essentials.Configs;
using AldursLab.WurmAssistant.Launcher.Core;

namespace AldursLab.WurmAssistant.Launcher
{
    public partial class MainForm : Form, IGuiHost
    {
        public MainForm()
        {
            InitializeComponent();
            ShowInTaskbar = true;
        }

        private void MainForm_Load(object sender, System.EventArgs e)
        {
            HideHostWindow();

            var assemblyDir = Path.GetDirectoryName(this.GetType().Assembly.Location);
            if (assemblyDir == null)
            {
                throw new NullReferenceException("assemblyDir is null");
            }

            string configFileName = "debug.cfg";
#if WA3STABLE
            configFileName = "wa3-stable.cfg";
#endif
#if WALITESTABLE
            configFileName = "walite-stable.cfg";
#endif

            var settingsFile = Path.Combine(assemblyDir, configFileName);

            IConfig localSettings = new FileSimpleConfig(settingsFile);
            this.Text = localSettings.GetValue("AppName") + " Launcher";

            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var rootDir = Path.Combine(localAppData, "AldursLab", localSettings.GetValue("AldursLabDirName"));

            var config = new ControllerConfig()
            {
                RootDirFullPath = rootDir,
                WebServiceRootUrl = localSettings.GetValue("WebServiceRootUrl"),
                WurmAssistantExeFileName = localSettings.GetValue("WurmAssistantExeFileName")
            };

            var controller = new LaunchController(this, config);
            controller.Execute();
        }

        public void ShowHostWindow()
        {
            var showing = !Visible;
            Visible = true;
            ShowInTaskbar = true;
            if (showing)
            {
                this.WindowState = FormWindowState.Normal;
                this.BringToFront();
            }
        }

        public void HideHostWindow()
        {
            Visible = false;
            ShowInTaskbar = false;
        }

        public void SetContent(UserControl userControl)
        {
            panel.Controls.Clear();
            userControl.Dock = DockStyle.Fill;
            panel.Controls.Add(userControl);
        }
    }
}
