using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AldurSoft.WurmAssistant3.Modules
{
    public interface IModule
    {
        ///// <summary>
        ///// Instructs the module to initialize its internals.
        ///// </summary>
        //void Initialize();

        /// <summary>
        /// Instructs the module to update its status. Interval is arbitrary.
        /// </summary>
        void Update();

        /// <summary>
        /// Instructs the module to stop.
        /// </summary>
        /// <param name="crashing">Indicates, if request to stop is due to an environment crashing.</param>
        void Stop(bool crashing);

        /// <summary>
        /// Asynchronously initializes the module, returns once initialization is completed.
        /// </summary>
        Task InitAsync();

        /// <summary>
        /// Id of this module.
        /// </summary>
        ModuleId Id { get; }
    }
}
