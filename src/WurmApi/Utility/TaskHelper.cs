using System;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace AldursLab.WurmApi.Utility
{
    static class TaskHelper
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
                    // for some reason, OperationCancelledException is converted to TaskCancelledException
                    // when accessing .Result of an async method Task.
                    var taskCancelledException = exception.InnerException as TaskCanceledException;
                    if (taskCancelledException != null)
                    {
                        throw new OperationCanceledException("Operation cancelled", taskCancelledException);
                    }
                    else
                    {
                        ExceptionDispatchInfo.Capture(exception.InnerException).Throw();
                    }
                }
                throw;
            }
        }
    }
}
