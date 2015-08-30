using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AldursLab.Essentials.Synchronization;
using AldursLab.PersistentObjects;
using AldursLab.PersistentObjects.FlatFiles;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Core;
using AldursLab.WurmAssistant3.Core.Infrastructure;
using AldursLab.WurmAssistant3.Core.Logging;
using AldursLab.WurmAssistantLite.Bootstrapping.Persistent;
using AldursLab.WurmAssistantLite.Views;
using JetBrains.Annotations;
using Ninject;

namespace AldursLab.WurmAssistantLite.Bootstrapping
{
    class AppBootstrapper : IDisposable
    {
        SharedDataDirectory dataDirectory;

        readonly MainForm mainForm;
        readonly Environment environment;
        readonly IKernel kernel = new StandardKernel();
        CoreBootstrapper coreBootstrapper;

        PersistentCollectionsLibrary persistentLibrary;
        Logger globalLogger;

        public AppBootstrapper([NotNull] MainForm mainForm)
        {
            if (mainForm == null) throw new ArgumentNullException("mainForm");
            this.mainForm = mainForm;
            environment = new Environment(mainForm);
        }

        bool Bootstrapped { get; set; }

        public void Bootstrap()
        {
            var marshaller = new WinFormsMainThreadEventMarshaller(mainForm);

            try
            {
                dataDirectory = new SharedDataDirectory();
            }
            catch (LockFailedException)
            {
                // must have exclusive lock on data directory, else most likely another app instance is using it
                //mainForm.Close();
                Application.Exit();
                return;
            }

            persistentLibrary = new PersistentCollectionsLibrary(new FlatFilesPersistenceStrategy(
                Path.Combine(dataDirectory.FullName, "WurmAssistantLiteSettings")));

            var globalSettings =
                new WurmAssistantLiteSettings(
                    persistentLibrary.DefaultCollection.GetObject<GlobalSettingsEntity>("GlobalSettings"))
                {
                    DataDirectoryFullPath = dataDirectory.FullName
                };

            var configurator = new Configurator(globalSettings);

            var valid = configurator.ExecConfig();
            if (!valid)
            {
                // stopping construction and exiting
                //mainForm.Close();
                Application.Exit();
                return;
            }

            var wurmAssistantConfig = configurator.BuildWurmAssistantConfig();

            // bind app specific dependencies
            kernel.Bind<WurmAssistantLiteSettings>().ToConstant(globalSettings);
            kernel.Bind<IWurmAssistantConfig, WurmAssistantConfig>().ToConstant(wurmAssistantConfig);
            kernel.Bind<IEventMarshaller, WinFormsMainThreadEventMarshaller>().ToConstant(marshaller);
            kernel.Bind<IEnvironment, Environment>().ToConstant(environment);

            // bootstrap core
            coreBootstrapper = new CoreBootstrapper(kernel);

            var appStartViewModel = coreBootstrapper.GetAppStartViewModel();
            var appStartView = new AppStartView(appStartViewModel);
            mainForm.SetAppCoreView(appStartView);

            var logOutputViewModel = appStartViewModel.LogOutputViewModel;
            var logOutputTempView = new LogOutputView(logOutputViewModel);
            mainForm.SetLogOutputView(logOutputTempView); 

            coreBootstrapper.BootstrapRuntime();
            globalLogger = kernel.Get<LoggerFactory>().Create("WA-Lite");
            globalLogger.Info("Core bootstrapping completed");

            globalLogger.Info("Resolving application runtime");
            var appRunningViewModel = coreBootstrapper.GetAppRunningViewModel();
            var appRunningView = new AppRunningView(appRunningViewModel, globalSettings);
            mainForm.SetAppCoreView(appRunningView);
            globalLogger.Info("Application runtime resolved");

            Bootstrapped = true;
            // save last, so if configuration causes bootstrap crash, it does not get saved
            SaveSettings();
            globalLogger.Info("Settings saved");
        }

        private void SaveSettings()
        {
            if (persistentLibrary != null && Bootstrapped)
            {
                persistentLibrary.SaveChanged();
            }
        }

        public void Update()
        {
            SaveSettings();
        }

        public void Dispose()
        {
            environment.Closing = true;
            SaveSettings();
            if (coreBootstrapper != null) coreBootstrapper.Dispose();
            kernel.Dispose();
            // release exclusive lock on data directory
            if (dataDirectory != null) dataDirectory.Dispose();
        }
    }

    class WurmAssistantConfig : IWurmAssistantConfig
    {
        public string DataDirectoryFullPath { get; set; }
        public string WurmGameClientInstallDirectory { get; set; }
        public Platform RunningPlatform { get; set; }
    }
}
