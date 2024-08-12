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
        /// <remarks>For example, calling with count equal to 3 will create an enumeration like [delay, delay, delay]</remarks>
        /// <param name="delay">Delay</param>
        /// <param name="count">Number of delays</param>
        /// <returns>Enumeration containing delays</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
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
        /// <remarks>For example, calling with count equal to 3 and factor 2 will create an enumeration like [firstDelay, firstDelay*2, firstDelay*4]</remarks>
        /// <param name="firstDelay">First delay</param>
        /// <param name="count">Number of delays</param>
        /// <param name="factor">Factor, default is 2</param>
        /// <returns>Enumeration containing delays</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
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
        /// Gets a set of exponential delays with randomized deviation (jitter) to prevent peak loads
        /// </summary>
        /// <param name="medianFirstDelay">Median first delay</param>
        /// <param name="count">Number of delays</param>
        /// <param name="seed">Seed for additional jitter randomization</param>
        /// <param name="fastFirst">Flag indicating whether the first delay should be set to zero (fast)</param>
        /// <returns>Enumeration containing delays</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
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
