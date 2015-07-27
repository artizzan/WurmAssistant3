using System;

namespace AldurSoft.WurmApi
{
    public interface ILogger
    {
        /// <summary>
        /// Logs the specified message.
        /// Implementation must not throw any exceptions.
        /// </summary>
        /// <param name="level">The severity level.</param>
        /// <param name="message">The message or null if none.</param>
        /// <param name="source">The source of the message or null if no specific source.</param>
        /// <param name="exception">The exception object, if applicable. Null otherwise.</param>
        void Log(LogLevel level, string message, object source, Exception exception);
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
