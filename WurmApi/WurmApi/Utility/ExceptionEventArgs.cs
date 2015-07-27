using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace AldurSoft.WurmApi.Utility
{
    public class ExceptionEventArgs : EventArgs
    {
        public Exception Exception { get; private set; }

        public ExceptionEventArgs([NotNull] Exception exception)
        {
            Exception = exception;
        }
    }
}
