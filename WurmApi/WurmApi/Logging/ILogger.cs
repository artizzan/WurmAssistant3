using System;

namespace AldurSoft.WurmApi.Logging
{
    public interface ILogger
    {
        /// <summary>
        /// Logs the specified message.
        /// </summary>
        /// <param name="level">The severity level.</param>
        /// <param name="message">The message.</param>
        /// <param name="source">The source of the message.</param>
        void Log(LogLevel level, string message, object source);

        /// <summary>
        /// Logs the specified message.
        /// </summary>
        /// <param name="level">The severity level.</param>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception object, if applicable.</param>
        /// <param name="source">The source of the message.</param>
        void Log(LogLevel level, string message, Exception exception, object source);
    }

    public enum LogLevel
    {
        /// <summary>
        /// Diagnostic. For debugging.
        /// </summary>
        Diag,

        /// <summary>
        /// Information. Statement of an event.
        /// </summary>
        Info,

        /// <summary>
        /// Warning. Potential error.
        /// </summary>
        Warn,

        /// <summary>
        /// Error. Unexpected failure, application consistency or other aspects may be compromised.
        /// </summary>
        Error,

        /// <summary>
        /// Critical error, application is in inconsistent or unstable state.
        /// </summary>
        Fatal
    }
}
