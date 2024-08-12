using System.Diagnostics;

namespace Incendium.RetryPolicy
{
    public class RateGateTests
    {
        [Fact]
        public async Task Test_RateGate_LimitRequestsByOneThread()
        {
            // arrange
            const int callsPerTimeUnit = 5;
            const int callsCount = 15;

            var timeUnit = TimeSpan.FromSeconds(1);
            var expectedDuration = timeUnit * (callsCount - callsPerTimeUnit) / callsPerTimeUnit;

            var rateGate = new RateGate(occurrences: callsPerTimeUnit, timeUnit);

            var stopWatch = Stopwatch.StartNew();

            // act
            for (var i = 0; i < callsCount; ++i)
                await rateGate.WaitToProceedAsync();

            stopWatch.Stop();

            // asserts
            Assert.True(stopWatch.Elapsed >= expectedDuration);
        }
    }
}
