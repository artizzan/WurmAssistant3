using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace AldursLab.Essentials.Asynchronous
{
    /// <summary>
    /// Runs an async operation at most once at the time,
    /// reruns it automatically if there were more triggers after last run.
    /// Note: No thread safety implemented, for correct operation this class must be Triggered from one thread.
    /// </summary>
    public class TriggerableAsyncOperation
    {
        readonly Func<Task> operationFactory;
        bool running;
        bool pending;

        public TriggerableAsyncOperation([NotNull] Func<Task> operationFactory)
        {
            if (operationFactory == null) throw new ArgumentNullException("operationFactory");
            this.operationFactory = operationFactory;
        }

        public async void Trigger()
        {
            if (running)
            {
                pending = true;
            }
            else
            {
                try
                {
                    running = true;
                    await operationFactory();
                    running = false;

                    while (pending)
                    {
                        pending = false;
                        running = true;
                        await operationFactory();
                        running = false;
                    }
                }
                catch (Exception)
                {
                    running = false;
                    pending = false;
                    throw;
                }
            }
        }
    }
}
