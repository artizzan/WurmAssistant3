using System;

namespace AldursLab.WurmAssistant3.Systems
{
    public interface IEnvironmentLifecycle
    {
        EnvironmentStatus EnvironmentStatus { get; }

        /// <summary>
        /// Event will be fired for both normal and unexpected environment closing (for example unhandled exceptions).
        /// To determine exact enviroment status, use <see cref="EnvironmentStatus"/>.
        /// </summary>
        event EventHandler EnvironmentClosing;

        void RestartEnvironment();
    }

    public enum EnvironmentStatus
    {
        Unspecified=0,
        /// <summary>
        /// Environment is initializing
        /// </summary>
        Initializing,
        /// <summary>
        /// Environment is binding dependencies
        /// </summary>
        Binding,
        /// <summary>
        /// Environment is configuring itself
        /// </summary>
        Configuring,
        /// <summary>
        /// Environment is starting itself
        /// </summary>
        Starting,
        /// <summary>
        /// Environment is running
        /// </summary>
        Running,
        /// <summary>
        /// Environment begins to shut down as part of normal lifecycle (ex. user closing)
        /// </summary>
        Stopping,
        /// <summary>
        /// Environment received an unhandled exception
        /// </summary>
        Crashing
    }
}
