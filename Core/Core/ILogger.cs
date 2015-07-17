using System;

namespace Aldurcraft.Core
{
    public interface ILogger
    {
        /// <summary>
        /// Logs the specified message.
        /// </summary>
        /// <param name="level">The severity level.</param>
        /// <param name="message">The message.</param>
        void Log(LogLevel level, string message);

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

        /// <summary>
        /// Logs the specified message with type Diag.
        /// </summary>
        /// <param name="message">The message.</param>
        void Diag(string message);

        /// <summary>
        /// Logs the specified message with type Diag.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="source">The source.</param>
        void Diag(string message, object source);

        /// <summary>
        /// Logs the specified message with type Diag.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        void Diag(string message, Exception exception);

        /// <summary>
        /// Logs the specified message with type Diag.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="source">The source.</param>
        void Diag(string message, Exception exception, object source);


        /// <summary>
        /// Logs the specified message with type Info.
        /// </summary>
        /// <param name="message">The message.</param>
        void Info(string message);

        /// <summary>
        /// Logs the specified message with type Info.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="source">The source.</param>
        void Info(string message, object source);

        /// <summary>
        /// Logs the specified message with type Info.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        void Info(string message, Exception exception);

        /// <summary>
        /// Logs the specified message with type Info.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="source">The source.</param>
        void Info(string message, Exception exception, object source);


        /// <summary>
        /// Logs the specified message with type Warn.
        /// </summary>
        /// <param name="message">The message.</param>
        void Warn(string message);

        /// <summary>
        /// Logs the specified message with type Warn.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="source">The source.</param>
        void Warn(string message, object source);

        /// <summary>
        /// Logs the specified message with type Warn.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        void Warn(string message, Exception exception);

        /// <summary>
        /// Logs the specified message with type Warn.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="source">The source.</param>
        void Warn(string message, Exception exception, object source);


        /// <summary>
        /// Logs the specified message with type Error.
        /// </summary>
        /// <param name="message">The message.</param>
        void Error(string message);

        /// <summary>
        /// Logs the specified message with type Error.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="source">The source.</param>
        void Error(string message, object source);

        /// <summary>
        /// Logs the specified message with type Error.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        void Error(string message, Exception exception);

        /// <summary>
        /// Logs the specified message with type Error.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="source">The source.</param>
        void Error(string message, Exception exception, object source);


        /// <summary>
        /// Logs the specified message with type Critical.
        /// </summary>
        /// <param name="message">The message.</param>
        void Critical(string message);

        /// <summary>
        /// Logs the specified message with type Critical.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="source">The source.</param>
        void Critical(string message, object source);

        /// <summary>
        /// Logs the specified message with type Critical.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        void Critical(string message, Exception exception);

        /// <summary>
        /// Logs the specified message with type Critical.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="source">The source.</param>
        void Critical(string message, Exception exception, object source);
    }

    public enum LogLevel
    {
        /// <summary>
        /// Diagnostic. For debugging.
        /// </summary>
        Diagnostic,

        /// <summary>
        /// Information. Statement of an event.
        /// </summary>
        Information,

        /// <summary>
        /// Warning. Potential error.
        /// </summary>
        Warning,

        /// <summary>
        /// Error. Unexpected failure, application consistency or other aspects may be compromised.
        /// </summary>
        Error,

        /// <summary>
        /// Critical error, application is in inconsistent or unstable state.
        /// </summary>
        Critical
    }
}
