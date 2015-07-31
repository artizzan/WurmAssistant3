using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;

namespace AldurSoft.WurmApi.Utility
{
    public static class TaskHelper
    {
        public static void UnwrapSingularAggegateException(Action action)
        {
            try
            {
                action();
            }
            catch (AggregateException exception)
            {
                if (exception.InnerExceptions.Count == 1)
                {
                    ExceptionDispatchInfo.Capture(exception.InnerException).Throw();
                }
                throw;
            }
        }

        public static T UnwrapSingularAggegateException<T>(Func<T> func)
        {
            try
            {
                return func();
            }
            catch (AggregateException exception)
            {
                if (exception.InnerExceptions.Count == 1)
                {
                    ExceptionDispatchInfo.Capture(exception.InnerException).Throw();
                }
                throw;
            }
        }
    }
}
