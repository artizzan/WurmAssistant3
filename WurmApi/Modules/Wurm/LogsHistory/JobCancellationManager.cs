using System;
using System.Threading;

namespace AldursLab.WurmApi.Modules.Wurm.LogsHistory
{
    class JobCancellationManager
    {
        CancellationToken internalCancellationToken;
        CancellationToken externalCancellationToken;

        public JobCancellationManager(CancellationToken internalCancellationToken, CancellationToken externalCancellationToken)
        {
            this.internalCancellationToken = internalCancellationToken;
            this.externalCancellationToken = externalCancellationToken;
        }

        /// <summary>
        /// Check for job cancellation and throws <see cref="OperationCanceledException"/>.
        /// These exceptions must be allowed to bubble to the job runner, to properly signal cancellation.
        /// </summary>
        public void ThrowIfCancelled()
        {
            externalCancellationToken.ThrowIfCancellationRequested();
            internalCancellationToken.ThrowIfCancellationRequested();
        }

        public CancellationToken GetLinkedToken()
        {
            return CancellationTokenSource.CreateLinkedTokenSource(externalCancellationToken, internalCancellationToken).Token;
        }
    }
}