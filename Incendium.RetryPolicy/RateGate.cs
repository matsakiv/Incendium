using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Incendium.RetryPolicy.Abstract;

namespace Incendium.RetryPolicy
{
    /// <summary>
    /// Represents a class that controls the number of requests in a given time unit
    /// </summary>
    public class RateGate : IDisposable, IRateGate
    {
        private readonly SemaphoreSlim _semaphore;
        private readonly ConcurrentQueue<int> _exitTimes;
        private readonly Timer _exitTimer;
        private bool _isDisposed;

        /// <summary>
        /// Number of requests per time unit
        /// </summary>
        public int Occurrences { get; private set; }
        /// <summary>
        /// Time unit in milliseconds
        /// </summary>
        public int TimeUnitMilliseconds { get; private set; }

        /// <summary>
        /// Initialize a new instance of the RateGate with the number of requests and time unit
        /// </summary>
        /// <param name="occurrences">Number of requests per time unit</param>
        /// <param name="timeUnit">Time unit</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public RateGate(int occurrences, TimeSpan timeUnit)
        {
            if (occurrences <= 0)
                throw new ArgumentOutOfRangeException(nameof(occurrences), "Number of occurrences must be a positive integer");
            if (timeUnit != timeUnit.Duration())
                throw new ArgumentOutOfRangeException(nameof(timeUnit), "Time unit must be a positive span of time");
            if (timeUnit >= TimeSpan.FromMilliseconds(uint.MaxValue))
                throw new ArgumentOutOfRangeException(nameof(timeUnit), "Time unit must be less than 2^32 milliseconds");

            Occurrences = occurrences;
            TimeUnitMilliseconds = (int)timeUnit.TotalMilliseconds;

            _semaphore = new SemaphoreSlim(Occurrences, Occurrences);
            _exitTimes = new ConcurrentQueue<int>();
            _exitTimer = new Timer(ExitTimerCallback, null, TimeUnitMilliseconds, -1);
        }

        private void ExitTimerCallback(object? state)
        {
            try
            {
                var exitTimeValid = _exitTimes.TryPeek(out int exitTime);

                while (exitTimeValid)
                {
                    if (unchecked(exitTime - Environment.TickCount) > 0)
                        break;

                    _semaphore.Release();
                    _exitTimes.TryDequeue(out exitTime);

                    exitTimeValid = _exitTimes.TryPeek(out exitTime);
                }

                var timeUntilNextCheck = exitTimeValid
                    ? Math.Min(TimeUnitMilliseconds, Math.Max(0, exitTime - Environment.TickCount))
                    : TimeUnitMilliseconds;

                _exitTimer.Change(timeUntilNextCheck, -1);
            }
            catch (Exception)
            {
                // can throw if called when disposing
            }
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="OperationCanceledException"></exception>
        public async Task<bool> WaitToProceedAsync(
            int millisecondsTimeout,
            CancellationToken cancellationToken = default)
        {
            if (millisecondsTimeout < -1)
                throw new ArgumentOutOfRangeException(nameof(millisecondsTimeout));

            CheckDisposed();

            var entered = await _semaphore.WaitAsync(millisecondsTimeout, cancellationToken);

            if (entered)
            {
                var timeToExit = unchecked(Environment.TickCount + TimeUnitMilliseconds);
                _exitTimes.Enqueue(timeToExit);
            }

            return entered;
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="OperationCanceledException"></exception>
        public Task<bool> WaitToProceedAsync(
            TimeSpan timeout,
            CancellationToken cancellationToken = default)
        {
            return WaitToProceedAsync((int)timeout.TotalMilliseconds, cancellationToken);
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="OperationCanceledException"></exception>
        public async Task WaitToProceedAsync(CancellationToken cancellationToken = default)
        {
            await WaitToProceedAsync(Timeout.Infinite, cancellationToken);
        }

        private void CheckDisposed()
        {
            if (_isDisposed)
                throw new ObjectDisposedException("RateGate is already disposed");
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (!_isDisposed)
            {
                if (isDisposing)
                {
                    _semaphore.Dispose();
                    _exitTimer.Dispose();

                    _isDisposed = true;
                }
            }
        }
    }
}
