using System;
using System.IO;
using System.Windows.Forms;
using AldursLab.Essentials.Configs;
using AldursLab.WurmAssistant.Launcher.Core;
using AldursLab.WurmAssistant.Launcher.Properties;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant.Launcher
{
    public partial class MainForm : Form, IGuiHost
    {
        readonly string[] args;
        readonly bool attemptBetaIfNoStable = false;

        public MainForm(string[] args)
        {
            this.args = args ?? new string[0];

            if (this.args.Length == 0)
            {
                // if no explicit arguments, choose default release channel for this version
#if CLICKONCEWO
                this.args = new[] { "stable-win" };
                attemptBetaIfNoStable = true;
#elif CLICKONCEWU
                this.args = new[] { "stable-win", "-WurmUnlimited" };
                attemptBetaIfNoStable = true;
#endif
            }

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

            var wurmUnlimitedMode =
                args.Length > 1 && args[1].Equals("-WurmUnlimited", StringComparison.InvariantCultureIgnoreCase);

            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string rootDir;

            if (wurmUnlimitedMode)
            {
                this.Text = "Wurm Assistant Unlimited Launcher";
                this.Icon = Resources.WurmAssistantUnlimitedIcon;
                rootDir = Path.Combine(localAppData, "AldursLab", "WurmAssistant3Unlimited");
            }
            else
            {
                this.Text = "Wurm Assistant 3 Launcher";
                rootDir = Path.Combine(localAppData, "AldursLab", "WurmAssistant3");
            }

            var config = new ControllerConfig()
            {
                RootDirFullPath = rootDir,
                WebServiceRootUrl = Properties.Settings.Default.WurmAssistantWebServiceUrl,
                WurmAssistantExeFileName = "AldursLab.WurmAssistant3.exe",
                BuildCode = args.Length > 0 ? args[0] : "stable-win",
                WurmUnlimitedMode = wurmUnlimitedMode,
                AttemptBetaIfNoStable = attemptBetaIfNoStable
            };

            IDebug debug = new TextDebug(Path.Combine(config.RootDirFullPath, "Launcher", "debug.txt"));
            debug.Clear();

            var controller = new LaunchController(this, config, debug);
            controller.Execute();
        }

        public void ShowHostWindow()
        {
            this.WindowState = FormWindowState.Normal;
            this.Refresh();
            //var showing = !Visible;
            //Visible = true;
            //ShowInTaskbar = true;
            //if (showing)
            //{
            //    this.WindowState = FormWindowState.Normal;
            //    this.BringToFront();
            //}
        }

        public void HideHostWindow()
        {
            this.WindowState = FormWindowState.Minimized;
            //Visible = false;
            //ShowInTaskbar = false;
        }

        public void SetContent(UserControl userControl)
        {
            //panel.Controls.Clear();
            userControl.Dock = DockStyle.Fill;
            panel.Controls.Add(userControl);
        }
    }
}
