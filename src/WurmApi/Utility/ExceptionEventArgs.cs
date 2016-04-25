using System;
using JetBrains.Annotations;

namespace AldursLab.WurmApi.Utility
{
    class ExceptionEventArgs : EventArgs
    {
        public Exception Exception { get; private set; }

        public ExceptionEventArgs([NotNull] Exception exception)
        {
            Exception = exception;
        }
    }
}
