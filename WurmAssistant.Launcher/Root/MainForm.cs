using System;
using System.IO;
using System.Windows.Forms;
using AldursLab.WurmAssistant.Launcher.Contracts;
using AldursLab.WurmAssistant.Launcher.Dto;
using AldursLab.WurmAssistant.Launcher.Modules;
using AldursLab.WurmAssistant.Launcher.Properties;
using AldursLab.WurmAssistant.Launcher.Views;

namespace AldursLab.WurmAssistant.Launcher.Root
{
    public partial class MainForm : Form, IGuiHost
    {
        readonly ArgsManager args;

        public MainForm(string[] args)
        {
            args = args ?? new string[0];
            this.args = new ArgsManager(args);

            InitializeComponent();
            ShowInTaskbar = true;
        }

        private void MainForm_Load(object sender, System.EventArgs e)
        {
            try
            {
                HideHostWindow();

                var assemblyDir = Path.GetDirectoryName(this.GetType().Assembly.Location);
                if (assemblyDir == null)
                {
                    throw new NullReferenceException("assemblyDir is null");
                }

                var config = new ControllerConfig()
                {
                    LauncherBinDirFullPath = assemblyDir,
                    WebServiceRootUrl = Properties.Settings.Default.WurmAssistantWebServiceUrl2,
                    WurmAssistantExeFileName = "AldursLab.WurmAssistant3.exe",
                    BuildCode = args.HasBuildCode ? args.BuildCode : string.Empty,
                    WurmUnlimitedMode = args.WurmUnlimitedMode,
                    UseRelativeDataDirPath = args.UseRelativeWaDataDir,
                    BuildNumber = args.HasSpecificBuildNumber ? args.SpecificBuildNumber.ToString() : null
                };

                if (args.NoArgs || args.ShowConfigWindow)
                {
                    var settings = new UserSettings(config);
                    // ask user what to run...
                    ChooseApp dialog = new ChooseApp(new WurmAssistantService(config.WebServiceRootUrl), settings);
                    dialog.StartPosition = FormStartPosition.CenterScreen;
                    if (dialog.ShowDialog() != DialogResult.OK)
                    {
                        Close();
                        return;
                    }

                    config.UseRelativeDataDirPath = dialog.RelativeDataDirPath;
                    config.BuildNumber = dialog.HasSpecificBuildNumber ? dialog.SpecificBuildNumber.ToString() : null;
                    config.WurmUnlimitedMode = dialog.RunUnlimited;
                    config.BuildCode = dialog.BuildCode;
                }

                if (string.IsNullOrWhiteSpace(config.BuildCode))
                {
                    throw new InvalidOperationException("Error: BuildCode must be known at this point.");
                }

                if (config.WurmUnlimitedMode)
                {
                    this.Text = "Launching Wurm Assistant Unlimited...";
                    this.Icon = Resources.WurmAssistantUnlimitedIcon;
                }
                else
                {
                    this.Text = "Launching Wurm Assistant 3...";
                }

                IDebug debug = new TextDebug(Path.Combine(assemblyDir, "launcherlog.txt"));
                debug.Clear();

                var controller = new LaunchController(this, config, debug);
                controller.Execute();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }

        public void ShowHostWindow()
        {
            this.WindowState = FormWindowState.Normal;
            this.Refresh();
        }

        public void HideHostWindow()
        {
            this.WindowState = FormWindowState.Minimized;
        }

        public void SetContent(UserControl userControl)
        {
            userControl.Dock = DockStyle.Fill;
            panel.Controls.Add(userControl);
        }
    }
}
