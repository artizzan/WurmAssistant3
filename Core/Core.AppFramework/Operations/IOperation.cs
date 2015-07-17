using System;
using System.Threading.Tasks;

namespace AldurSoft.Core.AppFramework.Operations
{
    public interface IOperation
    {
        /// <summary>
        /// Current status of the operation.
        /// </summary>
        OperationStatus Status { get; }

        /// <summary>
        /// Indicates if operation finished, regardless of result.
        /// </summary>
        bool Finished { get; }

        /// <summary>
        /// Exact exception that caused operation to become faulted.
        /// Optional. 
        /// </summary>
        Exception Error { get; }

        /// <summary>
        /// Returns descriptive text of current operation state.
        /// </summary>
        string CurrentState { get; }

        /// <summary>
        /// Begin execution of the operation
        /// </summary>
        void Execute();

        /// <summary>
        /// Instructs operation to cancel, if possible.
        /// </summary>
        void Cancel();

        /// <summary>
        /// Returns current progress of the operation.
        /// </summary>
        Percent Progress { get; }

        /// <summary>
        /// Indicates, if operation does not support precise progress.
        /// </summary>
        bool IndeterminateProgress { get; }
    }

    public interface IAsyncOperation : IOperation
    {
        Task WhenFinishedAsync();

        bool Running { get; }
    }

    public interface IOperationWithResult<out TResult>
    {
        TResult GetResult();
    }

    public interface IAsyncOperationWithResult<out TResult> : IOperation, IOperationWithResult<TResult>
    {
    }
}
