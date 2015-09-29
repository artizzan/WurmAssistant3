using System;
using System.IO;
using System.Windows.Forms;
using AldursLab.Essentials.Configs;
using AldursLab.WurmAssistant.Launcher.Core;

namespace AldursLab.WurmAssistant.Launcher
{
    public partial class MainForm : Form, IGuiHost
    {
        readonly string[] args;

        public MainForm(string[] args)
        {
            this.args = args ?? new string[0];
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

            this.Text = "Wurm Assistant 3 Launcher";

            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var rootDir = Path.Combine(localAppData, "AldursLab", "WurmAssistant3");

            var config = new ControllerConfig()
            {
                RootDirFullPath = rootDir,
                WebServiceRootUrl = Properties.Settings.Default.WurmAssistantWebServiceUrl,
                WurmAssistantExeFileName = "AldursLab.WurmAssistant3.exe",
                BuildCode = args.Length > 0 ? args[0] : "stable-win",
                BuildNumber = args.Length > 1 ? args[1] : null
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
