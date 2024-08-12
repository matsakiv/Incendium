using System;

namespace Incendium.RetryPolicy.Common
{
    internal sealed class ConcurrentRandom(int? seed = null)
    {
        private static readonly Random _static_random = new();

        private readonly Random _random = !seed.HasValue
            ? _static_random
            : new Random(seed.Value);

        public double NextDouble()
        {
            lock (_random)
            {
                return _random.NextDouble();
            }
        }
    }
}
