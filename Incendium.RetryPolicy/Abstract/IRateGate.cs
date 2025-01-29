using System;
using System.Threading;
using System.Threading.Tasks;

namespace Incendium.RetryPolicy.Abstract
{
    /// <summary>
    /// Provides a mechanism to control the rate of execution of operations
    /// </summary>
    public interface IRateGate : IDisposable
    {
        /// <summary>
        /// Asynchronously waits to proceed with execution
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to cancel operation</param>
        /// <returns>The task object representing the asynchronous operation</returns>
        Task WaitToProceedAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously waits to proceed with execution for the specified timeout period
        /// </summary>
        /// <param name="millisecondsTimeout">Timeout in milliseconds</param>
        /// <param name="cancellationToken">A cancellation token to cancel operation</param>
        /// <returns>The task object representing the asynchronous operation</returns>
        Task<bool> WaitToProceedAsync(int millisecondsTimeout, CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously waits to proceed with execution for the specified time interval
        /// </summary>
        /// <param name="timeout">Timeout</param>
        /// <param name="cancellationToken">A cancellation token to cancel operation</param>
        /// <returns>The task object representing the asynchronous operation</returns>
        Task<bool> WaitToProceedAsync(TimeSpan timeout, CancellationToken cancellationToken = default);
    }
}