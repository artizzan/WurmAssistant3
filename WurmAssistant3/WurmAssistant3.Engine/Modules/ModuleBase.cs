using System;
using System.Threading.Tasks;

using AldurSoft.Core;
using AldurSoft.SimplePersist;
using AldurSoft.WurmAssistant3.Modules;
using AldurSoft.WurmAssistant3.Systems;

using JetBrains.Annotations;

namespace AldurSoft.WurmAssistant3.Engine.Modules
{
    public abstract class ModuleBase : IModule
    {
        private readonly IModuleEngine moduleEngine;
        private readonly IPersistentManager persistentManager;

        private DateTime lastSave = DateTime.MinValue;

        public ModuleBase(
            [NotNull] ModuleId moduleId, 
            [NotNull] IModuleEngine moduleEngine,
            [NotNull] IPersistentManager persistentManager)
        {
            if (moduleId == null) throw new ArgumentNullException("moduleId");
            if (moduleEngine == null) throw new ArgumentNullException("moduleEngine");
            if (persistentManager == null) throw new ArgumentNullException("persistentManager");
            this.Id = moduleId;
            this.moduleEngine = moduleEngine;
            this.persistentManager = persistentManager;
            
            moduleEngine.RegisterModule(this);
        }

        public ModuleId Id { get; private set; }

        public async Task InitAsync()
        {
            await OnInitAsync();
        }

        protected virtual async Task OnInitAsync()
        {
        }

        public void Update()
        {
            OnUpdate();

            DateTime localNow = Time.Clock.LocalNow;
            if (lastSave < localNow.AddSeconds(-5))
            {
                persistentManager.SaveAllChanged();
                lastSave = localNow;
            }
        }

        protected virtual void OnUpdate()
        {
        }

        public void Stop(bool crashing)
        {
            OnStop(crashing);

            persistentManager.SaveAllChanged();
        }

        /// <summary>
        /// </summary>
        /// <param name="crashing">Indicates that environment is stopping due to a crash.</param>
        protected virtual void OnStop(bool crashing)
        {
        }
    }
}
