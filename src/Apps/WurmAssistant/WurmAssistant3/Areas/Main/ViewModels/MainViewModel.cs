using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AldursLab.WurmAssistant3.Areas.Core;
using AldursLab.WurmAssistant3.Areas.Main.Data;
using AldursLab.WurmAssistant3.Properties;
using AldursLab.WurmAssistant3.Utils.Extensions;
using JetBrains.Annotations;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using Screen = Caliburn.Micro.Screen;

namespace AldursLab.WurmAssistant3.Areas.Main.ViewModels
{
    [KernelBind(BindingHint.Singleton)]
    class MainViewModel : Screen
    {
        readonly ISystemTrayContextMenu trayMenu;
        readonly IConsoleArgs consoleArgs;
        readonly IWaVersionInfoProvider waVersionInfoProvider;
        readonly MainForm mainForm;

        WindowState savedNonMinimizedWindowState;

        Visibility _visibility;
        WindowState _windowState;
        string _windowTitle;
        ImageSource _icon;
        WindowsFormsHost _winFormsContent;

        public MainViewModel(
            [NotNull] MainDataContext dataContext, 
            [NotNull] ISystemTrayContextMenu trayMenu,
            [NotNull] IConsoleArgs consoleArgs,
            [NotNull] IWaVersionInfoProvider waVersionInfoProvider,
            [NotNull] MainForm mainForm)
        {
            if (dataContext == null) throw new ArgumentNullException(nameof(dataContext));
            if (trayMenu == null) throw new ArgumentNullException(nameof(trayMenu));
            if (consoleArgs == null) throw new ArgumentNullException(nameof(consoleArgs));
            if (waVersionInfoProvider == null) throw new ArgumentNullException(nameof(waVersionInfoProvider));
            if (mainForm == null) throw new ArgumentNullException(nameof(mainForm));
            this.DataContext = dataContext;
            this.trayMenu = trayMenu;
            this.consoleArgs = consoleArgs;
            this.waVersionInfoProvider = waVersionInfoProvider;
            this.mainForm = mainForm;

            trayMenu.ShowMainWindowClicked += ShowRestore;
            trayMenu.ExitWurmAssistantClicked += (sender, eventArgs) => this.TryClose();

            savedNonMinimizedWindowState = WindowState.Normal;

            if (consoleArgs.WurmUnlimitedMode)
            {
                WindowTitle = "Wurm Assistant Unlimited";
                Icon = Resources.WurmAssistantUnlimitedIcon.ToImageSource();
            }
            else
            {
                WindowTitle = "Wurm Assistant";
                Icon = Resources.WurmAssistantIcon.ToImageSource();
            }

            WindowTitle += string.Format(" ({0})", waVersionInfoProvider.Get());

            mainForm.Dock = DockStyle.Fill;
            WinFormsContent = new WindowsFormsHost()
            {
                Child = mainForm,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };
        }

        void ShowRestore(object sender, EventArgs e)
        {
            Visibility = Visibility.Visible;
            WindowState = savedNonMinimizedWindowState;
        }

        public MainDataContext DataContext { get; }

        public double Width
        {
            get { return DataContext.MainWindow.Width; }
            set { DataContext.MainWindow.Width = value; }
        }

        public double Height
        {
            get { return DataContext.MainWindow.Height; }
            set { DataContext.MainWindow.Height = value; }
        }

        public Visibility Visibility
        {
            get { return _visibility; }
            set
            {
                if (value == _visibility) return;
                _visibility = value;
                NotifyOfPropertyChange();
            }
        }

        public WindowState WindowState
        {
            get { return _windowState; }
            set
            {
                if (value == _windowState) return;

                if (value == WindowState.Minimized && _windowState != WindowState.Minimized)
                {
                    savedNonMinimizedWindowState = _windowState;
                }

                _windowState = value;
                NotifyOfPropertyChange();
            }
        }

        public string WindowTitle
        {
            get { return _windowTitle; }
            set
            {
                if (value == _windowTitle) return;
                _windowTitle = value;
                NotifyOfPropertyChange();
            }
        }

        public ImageSource Icon
        {
            get { return _icon; }
            set
            {
                if (Equals(value, _icon)) return;
                _icon = value;
                NotifyOfPropertyChange();
            }
        }

        public WindowsFormsHost WinFormsContent
        {
            get { return _winFormsContent; }
            set
            {
                if (Equals(value, _winFormsContent)) return;
                _winFormsContent = value;
                NotifyOfPropertyChange();
            }
        }
    }
}
