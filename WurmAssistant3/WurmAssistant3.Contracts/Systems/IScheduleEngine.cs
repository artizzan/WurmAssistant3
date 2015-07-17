using System;

namespace AldurSoft.WurmAssistant3.Systems
{
    /// <summary>
    /// Provides methods to execute operations on the application UI thread.
    /// </summary>
    public interface IScheduleEngine
    {
        /// <summary>
        /// Registers an action to be called regularly, but not more often than maximumFrequency.
        /// Registration for same callback will replace previous one.
        /// </summary>
        void RegisterForUpdates(TimeSpan maximumFrequency, Action<ExecutionInfo> callback);
    }

    /// <summary>
    /// Provides information about current job execution, statistics and parameters.
    /// </summary>
    public class ExecutionInfo
    {
        public TimeSpan TimeSinceLastCallback { get; set; }
    }
}
