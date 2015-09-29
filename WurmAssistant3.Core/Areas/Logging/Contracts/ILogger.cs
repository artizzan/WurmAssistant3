using System;

namespace AldursLab.WurmAssistant3.Core.Areas.Logging.Contracts
{
    public interface ILogger
    {
        void Error(string message);
        void Error(Exception exception, string message);
        void Info(string message);
        void Info(Exception exception, string message);
    }
}