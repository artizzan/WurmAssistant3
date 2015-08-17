using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using AldursLab.WurmAssistant.LauncherCore;
using AldursLab.WurmAssistant3.Launcher.Properties;

namespace AldursLab.WurmAssistant3.Launcher
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

            var localSettings = new LocalLauncherSettings();
            this.Text = localSettings.GetSetting("AppName") + " Launcher";

            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var rootDir = Path.Combine(localAppData, "AldursLab", localSettings.GetSetting("AldursLabDirName"));

            var config = new ControllerConfig()
            {
                RootDirFullPath = rootDir,
                WebServiceRootUrl = localSettings.GetSetting("WebServiceRootUrl"),
                WurmAssistantExeFileName = localSettings.GetSetting("WurmAssistantExeFileName")
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

    class LocalLauncherSettings
    {
        Dictionary<string, string> settings = new Dictionary<string, string>(); 

        public LocalLauncherSettings()
        {
            var assemblyDir = Path.GetDirectoryName(this.GetType().Assembly.Location);
            if (assemblyDir == null)
            {
                throw new NullReferenceException("assemblyDir is null");
            }
            var settingsFile = Path.Combine(assemblyDir, "LauncherSettings.txt");
            var lines = File.ReadAllLines(settingsFile).Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
            ParseLines(lines);
        }

        void ParseLines(string[] lines)
        {
            foreach (var line in lines)
            {
                var delimitedIndex = line.IndexOf("=", StringComparison.InvariantCulture);
                var key = line.Substring(0, delimitedIndex).Trim();
                var value = line.Substring(delimitedIndex + 1).Trim();
                settings.Add(key, value);
            }
        }

        public string GetSetting(string key)
        {
            return settings[key];
        }
    }
}
