using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AldursLab.PersistentObjects;
using AldursLab.PersistentObjects.FlatFiles;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Core.Infrastructure;
using AldursLab.WurmAssistant3.Core.Logging;
using AldursLab.WurmAssistantLite.Bootstrapping.Persistent;
using AldursLab.WurmAssistantLite.Views;
using JetBrains.Annotations;
using Ninject;

namespace AldursLab.WurmAssistantLite.Bootstrapping
{
    class AppBootstrapper
    {
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

            var dataDirPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData),
                "AldursLab",
                "WurmAssistantLite",
                "Data");

            if (!Directory.Exists(dataDirPath))
            {
                Directory.CreateDirectory(dataDirPath);
            }

            
            persistentLibrary = new PersistentCollectionsLibrary(new FlatFilesPersistenceStrategy(
                Path.Combine(dataDirPath, "GlobalSettings")));

            var globalSettings =
                new WurmAssistantLiteSettings(
                    persistentLibrary.DefaultCollection.GetObject<GlobalSettingsEntity>("WurmAssistantLiteSettings"))
                {
                    DataDirectoryFullPath = dataDirPath
                };


            var configurator = new Configurator(globalSettings);

            var valid = configurator.ExecConfig();
            if (!valid)
            {
                // stopping construction and exiting
                mainForm.Close();
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

        public void Close()
        {
            environment.Closing = true;
            SaveSettings();
            if (coreBootstrapper != null)
            {
                coreBootstrapper.Close();
            }
            kernel.Dispose();
        }

        public void Update()
        {
            SaveSettings();
        }
    }

    class WurmAssistantConfig : IWurmAssistantConfig
    {
        public string DataDirectoryFullPath { get; set; }
        public string WurmGameClientInstallDirectory { get; set; }
        public Platform RunningPlatform { get; set; }
    }
}
