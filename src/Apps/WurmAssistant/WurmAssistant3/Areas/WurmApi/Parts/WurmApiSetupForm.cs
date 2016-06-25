using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using AldursLab.Essentials.Extensions.DotNet;
using AldursLab.WurmApi;
using AldursLab.WurmApi.Modules.Wurm.InstallDirectory;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.WurmApi.Parts
{
    public partial class WurmApiSetupForm : Form
    {
        readonly bool wurmUnlimitedMode;

        public WurmApiSetupForm([CanBeNull] string currentWurmInstallDirectory, bool wurmUnlimitedMode)
        {
            this.wurmUnlimitedMode = wurmUnlimitedMode;

            InitializeComponent();

            wurmOnlineClientDirPath.Text = currentWurmInstallDirectory ?? string.Empty;
            if (wurmOnlineClientDirPath.Text.IsNullOrEmpty() && !wurmUnlimitedMode)
            {
                try
                {
                    var dirPath = WurmClientInstallDirectory.AutoDetect();
                    wurmOnlineClientDirPath.Text = dirPath.FullPath;
                }
                catch (WurmGameClientInstallDirectoryValidationException exception)
                {
                    autodetectFailedLabel2.Visible = true;
                    autodetectFailedLabel2.Text = "Failed to autodetect Wurm game client installation directory. Please choose manually." 
                        + Environment.NewLine 
                        + exception.Message;
                }
            }
        }

        public string SelectedWurmInstallDirectory { get; set; }

        private void FirstTimeSetupView_Load(object sender, EventArgs e)
        {
            if (wurmUnlimitedMode)
            {
                this.Text = "Wurm Assistant 3 Unlimited - Wurm Api Setup";
                labelPathDescription.Text = "Choose directory, where Wurm Unlimited game client keeps player data, "
                                            + Environment.NewLine +
                                            @"eg: C:\Games\SteamLibrary\steamapps\common\Wurm Unlimited\WurmLauncher\PlayerFiles";
            }
            else
            {

                this.Text = "Wurm Assistant 3 - Wurm Api Setup";
                labelPathDescription.Text = "Choose directory, where Wurm Online game client is installed, "
                                            + Environment.NewLine +
                                            @"eg: C:\Games\wurm";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
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

            SelectedWurmInstallDirectory = wurmOnlineClientDirPath.Text;

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

            if (wurmUnlimitedMode)
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
