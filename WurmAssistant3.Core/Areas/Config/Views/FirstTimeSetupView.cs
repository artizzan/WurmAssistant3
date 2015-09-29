using System;
using System.IO;
using System.Windows.Forms;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Core.Areas.Config.Modules;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Core.Areas.Config.Views
{
    public partial class FirstTimeSetupView : Form
    {
        readonly WurmAssistantConfig wurmAssistantConfig;

        public FirstTimeSetupView([NotNull] WurmAssistantConfig wurmAssistantConfig)
        {
            if (wurmAssistantConfig == null) throw new ArgumentNullException("wurmAssistantConfig");
            this.wurmAssistantConfig = wurmAssistantConfig;

            InitializeComponent();

            wurmOnlineClientDirPath.Text = wurmAssistantConfig.WurmGameClientInstallDirectory;
            OperatingSystem = wurmAssistantConfig.RunningPlatform;
        }

        private Platform OperatingSystem
        {
            get
            {
                Platform result = Platform.Unknown;
                if (rbOsLinux.Checked) result = Platform.Linux;
                if (rbOsMac.Checked) result = Platform.Mac;
                if (rbOsWindows.Checked) result = Platform.Windows;
                return result;
            }
            set
            {
                if (value == Platform.Linux)
                    rbOsLinux.Checked = true;
                else if (value == Platform.Mac)
                    rbOsMac.Checked = true;
                else if (value == Platform.Windows)
                    rbOsWindows.Checked = true;
                else if (value == Platform.Unknown)
                {
                    rbOsLinux.Checked = rbOsMac.Checked = rbOsWindows.Checked = false;
                }
                else
                {
                    throw new InvalidOperationException("Unexpected Platform value: " + value);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var platform = OperatingSystem;
            if (platform == Platform.Unknown)
            {
                MessageBox.Show("Operating system must be selected");
                return;
            }

            var rooted = Path.IsPathRooted(wurmOnlineClientDirPath.Text);
            if (!rooted)
            {
                MessageBox.Show(@"Path must be absolute, eg. C:\games\wurm or /home/MyUbuntu/wurm");
                return;
            }
            var exists = Directory.Exists(wurmOnlineClientDirPath.Text);
            if (!exists)
            {
                MessageBox.Show("Directory does not exist");
                return;
            }

            if (wurmAssistantConfig.WurmGameClientInstallDirectory != wurmOnlineClientDirPath.Text)
            {
                wurmAssistantConfig.WurmGameClientInstallDirectory = wurmOnlineClientDirPath.Text;
                wurmAssistantConfig.DropAllWurmApiCachesToggle = true;
            }
            if (wurmAssistantConfig.RunningPlatform != OperatingSystem)
            {
                wurmAssistantConfig.RunningPlatform = OperatingSystem;
                wurmAssistantConfig.DropAllWurmApiCachesToggle = true;
            }
            

            this.DialogResult = DialogResult.OK;
        }

        private void btnFindWurmDir_Click(object sender, EventArgs e)
        {
            var result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                wurmOnlineClientDirPath.Text = folderBrowserDialog1.SelectedPath;
            }
        }
    }
}
