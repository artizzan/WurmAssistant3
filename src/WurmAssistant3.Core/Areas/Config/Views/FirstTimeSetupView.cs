using System;
using System.IO;
using System.Linq;
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

        private void FirstTimeSetupView_Load(object sender, EventArgs e)
        {
            if (wurmAssistantConfig.WurmUnlimitedMode)
            {
                this.Text = "Wurm Assistant 3 Unlimited - First Time Setup";
                labelPathDescription.Text = "Choose directory, where Wurm Unlimited game client keeps player data, "
                                            + Environment.NewLine +
                                            @"eg. Windows: C:\Games\SteamLibrary\steamapps\common\Wurm Unlimited\WurmLauncher\PlayerFiles";
            }
            else
            {

                this.Text = "Wurm Assistant 3 - First Time Setup";
                labelPathDescription.Text = "Choose directory, where Wurm Online game client is installed, "
                                            + Environment.NewLine +
                                            "eg. Ubuntu: /Home/MyUbuntu/wurm" + Environment.NewLine +
                                            @"Windows: C:\Games\wurm";
            }
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
            var confirmed = ConfirmDirectory(wurmOnlineClientDirPath.Text);
            if (!confirmed)
            {
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

        bool ConfirmDirectory(string directoryPath)
        {
            bool valid;
            string extraInfo = string.Empty;
            var dir = new DirectoryInfo(directoryPath);

            var configsDirExists = dir.GetDirectories()
                                      .Any(
                                          info =>
                                              info.Name.Equals("configs", StringComparison.InvariantCultureIgnoreCase));
            var playersDirExists = dir.GetDirectories()
                                      .Any(
                                          info =>
                                              info.Name.Equals("players", StringComparison.InvariantCultureIgnoreCase));
            var packsDirExists =
                dir.GetDirectories().Any(info => info.Name.Equals("packs", StringComparison.InvariantCultureIgnoreCase));

            var isProbablyWurmUnlimited = configsDirExists && playersDirExists && !packsDirExists;
            var isProbablyWurmOnline = configsDirExists && playersDirExists && packsDirExists;

            if (wurmAssistantConfig.WurmUnlimitedMode)
            {
                valid = isProbablyWurmUnlimited;
                if (isProbablyWurmOnline)
                    extraInfo = " (you have probably chosen Wurm Online directory, instead of Wurm Unlimited)";
            }
            else
            {
                valid = isProbablyWurmOnline;
                if (isProbablyWurmUnlimited)
                    extraInfo = " (you have probably chosen Wurm Unlimited directory, instead of Wurm Online)";
            }

            if (!valid)
            {
                valid = MessageBox.Show("This directory does not appear to be correct. Are you sure?" + extraInfo,
                "Ops!",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Exclamation) == DialogResult.Yes;
            }

            return valid;
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
