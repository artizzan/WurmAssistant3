using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AldurSoft.SimplePersist
{
    public interface ISimplePersistLogger
    {
        void Log(string message, Severity severity);
    }

    public enum Severity
    {
        Debug,
        Warning,
        Error
    }
}
