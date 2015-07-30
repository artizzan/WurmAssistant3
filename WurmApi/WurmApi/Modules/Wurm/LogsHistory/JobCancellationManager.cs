using System.Threading;

namespace AldurSoft.WurmApi.Modules.Wurm.LogsHistory
{
    class JobCancellationManager
    {
        readonly CancellationToken internalCancellationToken;
        readonly CancellationToken externalCancellationToken;

        public JobCancellationManager(CancellationToken internalCancellationToken, CancellationToken externalCancellationToken)
        {
            this.internalCancellationToken = internalCancellationToken;
            this.externalCancellationToken = externalCancellationToken;
        }

        public void ThrowIfCancelled()
        {
            if (externalCancellationToken.IsCancellationRequested)
            {
                throw new InternalJobCancellationException();
            }
            else if (internalCancellationToken.IsCancellationRequested)
            {
                throw new ExternalJobCancellationException();
            }
        }
    }
}