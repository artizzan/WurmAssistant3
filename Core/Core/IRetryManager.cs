using System;
using System.Threading.Tasks;

namespace AldursLab.Deprec.Core
{
    /// <summary>
    /// Defines methods, that allow transparent retry of operation, based on some heuristics.
    /// </summary>
    public interface IRetryManager
    {
        /// <summary>
        /// Compensates the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="retries">The number of retries.</param>
        /// <param name="retryDelay">The delay between each retry.</param>
        void Apply(Action action, int retries = 3, TimeSpan? retryDelay = null);

        /// <summary>
        /// Compensates the specified function.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="function">The function.</param>
        /// <param name="retries">The number of retries.</param>
        /// <param name="retryDelay">The delay between retries.</param>
        /// <returns></returns>
        T Apply<T>(Func<T> function, int retries = 3, TimeSpan? retryDelay = null);

        /// <summary>
        /// Compensates the specified async method.
        /// </summary>
        /// <param name="taskFactory">The task factory.</param>
        /// <param name="retries">The number of retries.</param>
        /// <param name="retryDelay">The delay between retries.</param>
        /// <returns></returns>
        Task ApplyAsync(
            Func<Task> taskFactory, int retries = 3, TimeSpan? retryDelay = null);

        /// <summary>
        /// Compensates the specified async method with return value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="taskFactory">The task factory.</param>
        /// <param name="retries">The number of retries.</param>
        /// <param name="retryDelay">The delay between retries.</param>
        /// <returns></returns>
        Task<T> ApplyAsync<T>(
            Func<Task<T>> taskFactory, int retries = 3, TimeSpan? retryDelay = null);
    }
}
