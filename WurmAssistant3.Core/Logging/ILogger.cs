using System;

namespace AldursLab.WurmAssistant3.Core.Logging
{
    public interface ILogger
    {
        void Error(string message);
        void Error(Exception exception, string message);
        void Info(string message);
        void Info(Exception exception, string message);
    }
}