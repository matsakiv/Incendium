using System;
using System.Collections.Generic;
using Incendium.RetryPolicy.Common;

namespace Incendium.RetryPolicy
{
    public static class Delays
    {
        /// <summary>
        /// Gets a set of constant delays which are equal to <paramref name="delay"/>
        /// </summary>
        /// <remarks>
        /// For example, calling with count equal to 3 will create an enumeration: [delay, delay, delay].
        /// If count is 0, returns an empty enumeration.
        /// </remarks>
        /// <param name="delay">The time interval between retries</param>
        /// <param name="count">Number of delays to generate</param>
        /// <returns>An enumeration containing constant delays</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when delay is negative or count is negative</exception>
        public static IEnumerable<TimeSpan> Constant(TimeSpan delay, int count)
        {
            if (delay < TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(delay), delay, "Should be >= 0ms");

            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), count, "Should be >= 0");

            if (count == 0)
                yield break;

            for (var i = 0; i < count; i++)
                yield return delay;
        }

        /// <summary>
        /// Gets a set of exponential delays starting with <paramref name="firstDelay"/> and multiplying them by a <paramref name="factor"/> 
        /// </summary>
        /// <remarks>
        /// For example, calling with count equal to 3 and factor 2 will create an enumeration: [firstDelay, firstDelay*2, firstDelay*4].
        /// If count is 0, returns an empty enumeration.
        /// </remarks>
        /// <param name="firstDelay">The initial time interval</param>
        /// <param name="count">Number of delays to generate</param>
        /// <param name="factor">Multiplication factor for each subsequent delay (must be greater than 0)</param>
        /// <returns>An enumeration containing exponentially increasing delays</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when firstDelay is negative, count is negative, or factor is less than or equal to 0</exception>
        public static IEnumerable<TimeSpan> Exponential(TimeSpan firstDelay, int count, double factor = 2)
        {
            if (firstDelay < TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(firstDelay), firstDelay, "Should be >= 0ms");

            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), count, "Should be >= 0");

            if (factor <= 0)
                throw new ArgumentOutOfRangeException(nameof(factor), factor, "Should be > 0");

            if (count == 0)
                yield break;

            for (var i = 0; i < count; i++)
            {
                yield return firstDelay;

                firstDelay *= factor;
            }
        }

        /// <summary>
        /// Gets a set of exponential delays with randomized deviation (jitter) to prevent peak loads using the Decorrelated Jitter algorithm
        /// </summary>
        /// <remarks>
        /// This implementation uses a modified version of the Decorrelated Jitter backoff algorithm to generate delays.
        /// The algorithm adds randomization to prevent thundering herd problems in distributed systems.
        /// If count is 0, returns an empty array.
        /// </remarks>
        /// <param name="medianFirstDelay">The median value for the first delay</param>
        /// <param name="count">Number of delays to generate</param>
        /// <param name="seed">Optional seed value for the random number generator to produce deterministic sequences</param>
        /// <param name="fastFirst">When true, the first retry will be immediate (zero delay)</param>
        /// <returns>An enumeration containing randomized exponential delays with jitter</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when medianFirstDelay is negative or count is negative</exception>
        public static IEnumerable<TimeSpan> DecorrelatedJitterBackoffV2(
            TimeSpan medianFirstDelay,
            int count,
            int? seed = null,
            bool fastFirst = false)
        {
            if (medianFirstDelay < TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(medianFirstDelay), medianFirstDelay, "Should be >= 0ms");

            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), count, "Should be >= 0");
 
            if (count == 0)
                return [];

            return Enumerate(medianFirstDelay, count, fastFirst, new ConcurrentRandom(seed));

            static IEnumerable<TimeSpan> Enumerate(TimeSpan scaleFirstTry, int maxRetries, bool fast, ConcurrentRandom random)
            {
                var maxValue = TimeSpan.MaxValue;
                var maxTimeSpanDouble = maxValue.Ticks - 1000.0;
                int i = 0;

                if (fast)
                {
                    i++;
                    yield return TimeSpan.Zero;
                }

                var targetTicksFirstDelay = scaleFirstTry.Ticks;
                var num = 0.0;

                for (; i < maxRetries; i++)
                {
                    var num2 = i + random.NextDouble();
                    var next = Math.Pow(2.0, num2) * Math.Tanh(Math.Sqrt(4.0 * num2));
                    var num3 = next - num;

                    yield return TimeSpan.FromTicks((long)Math.Min(num3 * 0.7142857142857143 * targetTicksFirstDelay, maxTimeSpanDouble));

                    num = next;
                }
            }
        }
    }
}
