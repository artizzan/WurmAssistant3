using System;
using System.Globalization;
using System.Windows.Forms;
using AldursLab.WurmAssistant.Launcher.Contracts;
using AldursLab.WurmAssistant.Launcher.Modules;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant.Launcher.Views
{
    public partial class ChooseApp : Form
    {
        readonly IWurmAssistantService wurmAssistantService;
        readonly UserSettings settings;
        LaunchChoices launchChoices;

        public ChooseApp([NotNull] IWurmAssistantService wurmAssistantService,
            [NotNull] UserSettings settings)
        {
            if (wurmAssistantService == null) throw new ArgumentNullException("wurmAssistantService");
            if (settings == null) throw new ArgumentNullException("settings");
            this.wurmAssistantService = wurmAssistantService;
            this.settings = settings;

            InitializeComponent();

            if (!string.IsNullOrWhiteSpace(settings.SpecificBuildNumber))
            {
                useSpecificBuildNumberCb.Checked = true;
                decimal value;
                if (decimal.TryParse(settings.SpecificBuildNumber, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                {
                    specificBuildNumberNb.Value = value;
                }
                else
                {
                    settings.SpecificBuildNumber = null;
                }
            }

            relativeDataDirPathCb.Checked = settings.UseRelativeDataDir;

            UpdateControls();

            timer1.Enabled = true;
        }

        void PreselectLastLaunchChoice(LaunchChoices lastLaunchChoice)
        {
            if (lastLaunchChoice.HasFlag(LaunchChoices.WurmUnlimited))
            {
                if (lastLaunchChoice.HasFlag(LaunchChoices.Dev))
                {
                    unlimDevBtn.Focus();
                }
                else if (lastLaunchChoice.HasFlag(LaunchChoices.Beta))
                {
                    unlimBetaBtn.Focus();
                }
                else if (lastLaunchChoice.HasFlag(LaunchChoices.StableWin))
                {
                    unlimStableWinBtn.Focus();
                }
                else if (lastLaunchChoice.HasFlag(LaunchChoices.StableLin))
                {
                    unlimStableLinBtn.Focus();
                }
                else if (lastLaunchChoice.HasFlag(LaunchChoices.StableMac))
                {
                    unlimStableMacBtn.Focus();
                }
            }
            else
            {
                if (lastLaunchChoice.HasFlag(LaunchChoices.Dev))
                {
                    devBtn.Focus();
                }
                else if (lastLaunchChoice.HasFlag(LaunchChoices.Beta))
                {
                    betaBtn.Focus();
                }
                else if (lastLaunchChoice.HasFlag(LaunchChoices.StableWin))
                {
                    stableWinBtn.Focus();
                }
                else if (lastLaunchChoice.HasFlag(LaunchChoices.StableLin))
                {
                    stableLinBtn.Focus();
                }
                else if (lastLaunchChoice.HasFlag(LaunchChoices.StableMac))
                {
                    stableMacBtn.Focus();
                }
            }

            launchChoices = lastLaunchChoice;
        }

        public bool RelativeDataDirPath { get { return relativeDataDirPathCb.Checked; } }

        public bool HasSpecificBuildNumber { get { return useSpecificBuildNumberCb.Checked; } }

        public int SpecificBuildNumber { get { return (int)specificBuildNumberNb.Value; } }

        public LaunchChoices LaunchChoices
        {
            get { return launchChoices; }
            private set
            {
                launchChoices = value;
                settings.LastLaunchChoice = value;
            }
        }

        public bool RunUnlimited { get { return LaunchChoices.HasFlag(LaunchChoices.WurmUnlimited); } }

        public string BuildCode
        {
            get
            {
                if (LaunchChoices.HasFlag(LaunchChoices.Dev))
                {
                    return "dev";
                }
                else if (LaunchChoices.HasFlag(LaunchChoices.Beta))
                {
                    return "beta";
                }
                else if (LaunchChoices.HasFlag(LaunchChoices.StableWin))
                {
                    return "stable-win";
                }
                else if (LaunchChoices.HasFlag(LaunchChoices.StableLin))
                {
                    return "stable-lin";
                }
                else if (LaunchChoices.HasFlag(LaunchChoices.StableMac))
                {
                    return "stable-mac";
                }
                else
                {
                    throw new NotSupportedException();
                }
            }
        }

        private void stableWinBtn_Click(object sender, EventArgs e)
        {
            LaunchChoices = LaunchChoices.StableWin;
            DialogResult = DialogResult.OK;
        }

        private void stableLinBtn_Click(object sender, EventArgs e)
        {
            LaunchChoices = LaunchChoices.StableLin;
            DialogResult = DialogResult.OK;
        }

        private void stableMacBtn_Click(object sender, EventArgs e)
        {
            LaunchChoices = LaunchChoices.StableMac;
            DialogResult = DialogResult.OK;
        }

        private void betaBtn_Click(object sender, EventArgs e)
        {
            LaunchChoices = LaunchChoices.Beta;
            DialogResult = DialogResult.OK;
        }

        private void devBtn_Click(object sender, EventArgs e)
        {
            LaunchChoices = LaunchChoices.Dev;
            DialogResult = DialogResult.OK;
        }

        private void unlimStableWinBtn_Click(object sender, EventArgs e)
        {
            LaunchChoices = LaunchChoices.StableWin | LaunchChoices.WurmUnlimited;
            DialogResult = DialogResult.OK;
        }

        private void unlimStableLinBtn_Click(object sender, EventArgs e)
        {
            LaunchChoices = LaunchChoices.StableLin | LaunchChoices.WurmUnlimited;
            DialogResult = DialogResult.OK;
        }

        private void unlimStableMacBtn_Click(object sender, EventArgs e)
        {
            LaunchChoices = LaunchChoices.StableMac | LaunchChoices.WurmUnlimited;
            DialogResult = DialogResult.OK;
        }

        private void unlimBetaBtn_Click(object sender, EventArgs e)
        {
            LaunchChoices = LaunchChoices.Beta | LaunchChoices.WurmUnlimited;
            DialogResult = DialogResult.OK;
        }

        private void unlimDevBtn_Click(object sender, EventArgs e)
        {
            LaunchChoices = LaunchChoices.Dev | LaunchChoices.WurmUnlimited;
            DialogResult = DialogResult.OK;
        }

        private void relativeDataDirPathCb_CheckedChanged(object sender, EventArgs e)
        {
            settings.UseRelativeDataDir = relativeDataDirPathCb.Checked;
        }

        private void useSpecificBuildNumberCb_CheckedChanged(object sender, EventArgs e)
        {
            if (!useSpecificBuildNumberCb.Checked)
            {
                settings.SpecificBuildNumber = null;
            }
            UpdateControls();
        }

        private void UpdateControls()
        {
            specificBuildNumberNb.Enabled = useSpecificBuildNumberCb.Checked;
        }

        private async void checkAllBtn_Click(object sender, EventArgs e)
        {
            try
            {
                checkAllBtn.Enabled = false;
                var packages = await wurmAssistantService.GetAllPackages();
                var view = new PackagesView(packages);
                view.Show();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                checkAllBtn.Enabled = true;
            }
        }

        private void specificBuildNumberNb_ValueChanged(object sender, EventArgs e)
        {
            settings.SpecificBuildNumber = specificBuildNumberNb.Value.ToString(CultureInfo.InvariantCulture);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            PreselectLastLaunchChoice(settings.LastLaunchChoice);
        }
    }
}
