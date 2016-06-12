using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using AldursLab.Essentials.Eventing;
using AldursLab.Essentials.Extensions.DotNet.IO;
using AldursLab.Essentials.Synchronization;
using AldursLab.PersistentObjects;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Areas.Calendar;
using AldursLab.WurmAssistant3.Areas.CombatAssistant;
using AldursLab.WurmAssistant3.Areas.Config;
using AldursLab.WurmAssistant3.Areas.Config.Contracts;
using AldursLab.WurmAssistant3.Areas.Core;
using AldursLab.WurmAssistant3.Areas.Core.Contracts;
using AldursLab.WurmAssistant3.Areas.Core.Services;
using AldursLab.WurmAssistant3.Areas.CraftingAssistant;
using AldursLab.WurmAssistant3.Areas.Features;
using AldursLab.WurmAssistant3.Areas.Features.Contracts;
using AldursLab.WurmAssistant3.Areas.Granger;
using AldursLab.WurmAssistant3.Areas.Logging;
using AldursLab.WurmAssistant3.Areas.Logging.Contracts;
using AldursLab.WurmAssistant3.Areas.LogSearcher;
using AldursLab.WurmAssistant3.Areas.MainMenu;
using AldursLab.WurmAssistant3.Areas.Native;
using AldursLab.WurmAssistant3.Areas.Native.Contracts;
using AldursLab.WurmAssistant3.Areas.Native.Services;
using AldursLab.WurmAssistant3.Areas.Persistence;
using AldursLab.WurmAssistant3.Areas.RevealCreatures;
using AldursLab.WurmAssistant3.Areas.SkillStats;
using AldursLab.WurmAssistant3.Areas.SoundManager;
using AldursLab.WurmAssistant3.Areas.Timers;
using AldursLab.WurmAssistant3.Areas.TrayPopups;
using AldursLab.WurmAssistant3.Areas.Triggers;
using AldursLab.WurmAssistant3.Areas.WurmApi;
using AldursLab.WurmAssistant3.Systems.AppUpgrades;
using AldursLab.WurmAssistant3.Systems.ConventionBinding;
using AldursLab.WurmAssistant3.Systems.Plugins;
using AldursLab.WurmAssistant3.Utils;
using AldursLab.WurmAssistant3.Utils.IoC;
using AldursLab.WurmAssistant3.Utils.WinForms.Reusables;
using Caliburn.Micro;
using Ninject;
using Ninject.Extensions.Factory;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace AldursLab.WurmAssistant3
{
    public class Bootstrapper : BootstrapperBase
    {
        readonly IKernel kernel = new StandardKernel();
        IConsoleArgs consoleArgs;

        public Bootstrapper()
        {
            Initialize();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            base.OnStartup(sender, e);

            try
            {
                consoleArgs = new ConsoleArgs();
                kernel.Bind<IConsoleArgs>().ToConstant(consoleArgs);

                var dataDirectory = new WurmAssistantDataDirectory(consoleArgs);
                kernel.Bind<IWurmAssistantDataDirectory, WurmAssistantDataDirectory>()
                      .ToConstant(dataDirectory);

                var pluginsDir = new DirectoryInfo(Path.Combine(dataDirectory.DirectoryPath, "Plugins"));
                var pluginManager = new PluginManager(pluginsDir);
                kernel.Bind<PluginManager>().ToConstant(pluginManager);

                VersionUpgradeManager upgradeManager = new VersionUpgradeManager(dataDirectory, consoleArgs);
                upgradeManager.RunUpgrades();

                System.Windows.Forms.Application.EnableVisualStyles();
                Regex.CacheSize = 1000;

                var kernelConfig = KernelConfig.EnableFor(kernel);
                kernel.Bind<IKernelConfig>().ToConstant(kernelConfig);

                IMessageBus messageBus = new MessageBus();
                kernel.Bind<IMessageBus>().ToConstant(messageBus);

                kernelConfig.AddPostInitializeActivations(
                    (context, reference) =>
                    {
                        if (reference.Instance is IHandle)
                        {
                            messageBus.Subscribe(reference.Instance);
                        }
                    },
                    (context, reference) =>
                    {
                        if (reference.Instance is IHandle)
                        {
                            messageBus.Unsubscribe(reference.Instance);
                        }
                    });

                var priorityBindingOrder = new List<string>()
                {
                    "Core",
                    "Logging",
                    "Persistence",
                };

                var conventionBindingManager = new ConventionBindingManager(
                    kernel, 
                    priorityBindingOrder,
                    new [] { this.GetType().Assembly }.Concat(pluginManager.PluginAssemblies).ToArray());
                conventionBindingManager.BindAreasByConvention();

                var featureManager = kernel.Get<IFeaturesManager>();
                featureManager.InitFeatures();

                var mainForm = kernel.Get<MainForm>();
                mainForm.Closed += (o, args) => ShutdownCurrentApp();
                mainForm.Show();
            }
            catch (LockFailedException)
            {
                try
                {
                    AttemptToRestoreAlreadyRunningAppInstance();
                }
                catch (Exception exception)
                {
                    ShowErrorAsDialog(exception);
                }

                ShutdownCurrentApp();
            }
            catch (ConfigCancelledException)
            {
                ShutdownCurrentApp();
            }
            catch (Exception exception)
            {
                bool handled = false;

                var validator = new IrrklangDependencyValidator();

                handled = validator.HandleWhenMissingIrrklangDependency(exception);

                if (!handled)
                {
                    ShowErrorAsDialog(exception);
                }

                ShutdownCurrentApp();
            }
        }

        void AttemptToRestoreAlreadyRunningAppInstance()
        {
            if (consoleArgs != null)
            {
                INativeCalls rwc = new Win32NativeCalls();
                if (consoleArgs.WurmUnlimitedMode)
                {
                    rwc.AttemptToBringMainWindowToFront("AldursLab.WurmAssistant3", "Unlimited");
                }
                else
                {
                    rwc.AttemptToBringMainWindowToFront("AldursLab.WurmAssistant3", @"^((?!Unlimited).)*$");
                }
            }
        }

        void ShowErrorAsDialog(Exception exception)
        {
            bool restart = false;
            var btn1 = new Button()
            {
                Text = "Reset Wurm Assistant config",
                Height = 28,
                Width = 220
            };
            btn1.Click += (o, args) =>
            {
                if (TryResetConfig())
                {
                    System.Windows.Forms.MessageBox.Show("Reset complete, please restart.", "Done", MessageBoxButtons.OK);
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Reset was not possible.", "Sorry", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
            var btn2 = new Button()
            {
                Text = "Restart Wurm Assistant",
                Height = 28,
                Width = 220,
                DialogResult = DialogResult.OK
            };
            btn2.Click += (o, args) => restart = true;
            var btn3 = new Button()
            {
                Text = "Close Wurm Assistant",
                Height = 28,
                Width = 220,
                DialogResult = DialogResult.OK
            };
            var view = new UniversalTextDisplayView(btn3, btn2, btn1)
            {
                Text = "OH NO!!",
                ContentText = "Application startup was interrupted by an ugly error! "
                              + Environment.NewLine
                              + Environment.NewLine + exception.ToString()
            };

            view.ShowDialog();
            if (restart) RestartCurrentApp();
            else ShutdownCurrentApp();
        }

        public bool TryResetConfig()
        {
            var settings = kernel.TryGet<IWurmAssistantConfig>();
            if (settings != null)
            {
                settings.WurmApiResetRequested = true;
                return true;
            }
            return false;
        }

        void ShutdownCurrentApp()
        {
            Application.Current.Shutdown();
            kernel.Dispose();
        }

        void RestartCurrentApp()
        {
            System.Windows.Forms.Application.Restart();
            // Restart does not automatically shutdown WPF application
            ShutdownCurrentApp();
        }

        #region Kernel wirings for view resolver

        protected override object GetInstance(Type service, string key)
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            var instance = key == null ? kernel.Get(service) : kernel.Get(service, key);

            if (instance == null)
            {
                throw new InvalidOperationException("dependency missing for type " + service.FullName);
            }

            return instance;
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            return kernel.GetAll(service);
        }

        protected override void BuildUp(object instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }
            kernel.Inject(instance);
        }

        #endregion
    }
}
