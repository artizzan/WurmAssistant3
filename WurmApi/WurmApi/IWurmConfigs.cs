using System;
using System.Collections.Generic;

namespace AldurSoft.WurmApi
{
    /// <summary>
    /// Provides means of working with wurm configs.
    /// </summary>
    public interface IWurmConfigs
    {
        /// <summary>
        /// Triggered, when config is edited in any way.
        /// </summary>
        event EventHandler<EventArgs> ConfigsChanged;

        /// <summary>
        /// Enumerates all configs detected and managed by WurmApi.
        /// </summary>
        IEnumerable<IWurmConfig> All { get; }
        
        /// <summary>
        /// Gets config by its name, case insensitive.
        /// Returns null, if config does not exist or cannot be read.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="DataNotFoundException"></exception>
        IWurmConfig GetConfig(string name);
    }
}