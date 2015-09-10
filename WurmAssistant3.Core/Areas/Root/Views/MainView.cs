using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AldursLab.Essentials.Synchronization;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Core.Areas.Logging.Views;
using AldursLab.WurmAssistant3.Core.Areas.ModuleManager.Views;
using AldursLab.WurmAssistant3.Core.Areas.Root.Model;
using AldursLab.WurmAssistant3.Core.Areas.Root.ViewModels;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Core.Areas.Root.Views
{
    public partial class MainView : Form, ITimerService, IHostEnvironment
    {
        readonly MainViewModel mainViewModel;
        readonly CoreBootstrapper bootstrapper;
        bool bootstrapped = false;

        public MainView([NotNull] MainViewModel mainViewModel)
        {
            if (mainViewModel == null) throw new ArgumentNullException("mainViewModel");
            this.mainViewModel = mainViewModel;
            InitializeComponent();

            mainViewModel.PropertyChanged += MainViewModelOnPropertyChanged;

            bootstrapper = new CoreBootstrapper(this, mainViewModel);
            bootstrapper.BootstrapCore();
            InitTimer.Enabled = true;
        }

        void MainViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName == "LogViewModel")
            {
                LogViewPanel.Controls.Clear();
                if (mainViewModel.LogViewModel != null)
                {
                    LogViewPanel.Controls.Add(new LogView(mainViewModel.LogViewModel) {Dock = DockStyle.Fill});
                }
            }
            else if (propertyChangedEventArgs.PropertyName == "MenuViewModel")
            {
                MenuViewPanel.Controls.Clear();
                if (mainViewModel.MenuViewModel != null)
                {
                    MenuViewPanel.Controls.Add(new MenuView(mainViewModel.MenuViewModel) { Dock = DockStyle.Fill });
                }
            }
            else if (propertyChangedEventArgs.PropertyName == "ModulesViewModel")
            {
                ModulesViewPanel.Controls.Clear();
                if (mainViewModel.ModulesViewModel != null)
                {
                    LogViewPanel.Controls.Add(new ModulesView(mainViewModel.ModulesViewModel) { Dock = DockStyle.Fill });
                }
            }
        }

        private void InitTimer_Tick(object sender, EventArgs e)
        {
            if (!bootstrapped)
            {
                InitTimer.Enabled = false;

                try
                {
                    bootstrapper.BootstrapRuntime();
                }
                catch (Exception exception)
                {
                    MessageBox.Show("Application must close due to exception: " + exception.ToString());
                }
                

                bootstrapped = true;
                UpdateTimer.Enabled = true;

                bootstrapper.RunAsyncInits();
            }
        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            // guard against running update on scheduled ticks, after beginning shutdown
            if (!AppClosing)
            {
                OnUpdate();
            }
        }

        public event EventHandler<EventArgs> Updated;

        protected virtual void OnUpdate()
        {
            var handler = Updated;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        private void MainView_FormClosing(object sender, FormClosingEventArgs e)
        {
            UpdateTimer.Enabled = false;
            // final update tick.
            OnUpdate();
            bootstrapper.Dispose();
        }

        public event EventHandler<EventArgs> HostClosing;

        bool AppClosing { get; set; }
        bool IHostEnvironment.Closing
        {
            get { return this.AppClosing; }
        }

        public void Restart()
        {
            AppClosing = true;
            OnHostClosing();
            Application.Restart();
        }

        public void Shutdown()
        {
            AppClosing = true;
            OnHostClosing();
            Application.Exit();
        }

        public Platform Platform { get { return Platform.Unknown;} }

        protected virtual void OnHostClosing()
        {
            var handler = HostClosing;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }
    }
}
