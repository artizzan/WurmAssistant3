using System;
using System.Collections.Generic;
using System.IO;

using AldurSoft.WurmApi;
using AldurSoft.WurmAssistant3.Modules;
using AldurSoft.WurmAssistant3.Systems;

using JetBrains.Annotations;

using NLog;

namespace AldurSoft.WurmAssistant3.Engine.Systems
{
    public class ModuleEngine : IModuleEngine
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IWurmApiController wurmApiController;
        private readonly IEnvironment environment;
        private readonly IScheduleEngine scheduleEngine;
        private readonly IEnvironmentLifecycle environmentLifecycle;
        private readonly Dictionary<ModuleId, ModuleContainer> modules = new Dictionary<ModuleId, ModuleContainer>();

        private bool enviromentClosing;

        public ModuleEngine(
            [NotNull] IWurmApiController wurmApiController,
            [NotNull] IEnvironment environment,
            [NotNull] IScheduleEngine scheduleEngine,
            [NotNull] IEnvironmentLifecycle environmentLifecycle)
        {
            if (wurmApiController == null) throw new ArgumentNullException("wurmApiController");
            if (environment == null) throw new ArgumentNullException("environment");
            if (scheduleEngine == null) throw new ArgumentNullException("scheduleEngine");
            if (environmentLifecycle == null) throw new ArgumentNullException("environmentLifecycle");
            this.wurmApiController = wurmApiController;
            this.environment = environment;
            this.scheduleEngine = scheduleEngine;
            this.environmentLifecycle = environmentLifecycle;

            scheduleEngine.RegisterForUpdates(TimeSpan.FromMilliseconds(500), Update);
            environmentLifecycle.EnvironmentClosing += EnvironmentLifecycleOnEnvironmentClosing;
        }

        public void RegisterModule([NotNull] IModule module)
        {
            if (module == null)
            {
                throw new ArgumentNullException("module");
            }

            if (modules.ContainsKey(module.Id))
            {
                throw new InvalidOperationException(
                    string.Format("module with id {0} is already registered", module.Id));
            }
            modules.Add(module.Id, new ModuleContainer()
            {
                Module = module,
                Initialized = false
            });
            InitModule(module);
        }

        async void InitModule(IModule module)
        {
            try
            {
                await module.InitAsync();
                modules[module.Id].Initialized = true;
            }
            catch (Exception)
            {
                Logger.Error(string.Format("Module {0} failed to initialize.", module.Id));
            }
        }

        private void Update(ExecutionInfo executionInfo)
        {
            if (enviromentClosing)
            {
                Logger.Info("Environment is closing, skipping update");
                return;
            }
            wurmApiController.Update();
            foreach (var container in modules.Values)
            {
                if (container.Initialized)
                {
                    container.Module.Update();
                }
            }
        }

        private void EnvironmentLifecycleOnEnvironmentClosing(object sender, EventArgs eventArgs)
        {
            enviromentClosing = true;
            foreach (var container in modules.Values)
            {
                container.Module.Stop(environmentLifecycle.EnvironmentStatus == EnvironmentStatus.Crashing);
            }
        }

        class ModuleContainer
        {
            public IModule Module { get; set; }
            public bool Initialized { get; set; }
        }
    }
}
