using System.Diagnostics;
using System.Net;

namespace Incendium.RetryPolicy
{
    public class RetryHttpClientHandlerTests
    {
        private class MockHttpClientHandler : HttpMessageHandler
        {
            public const string MESSAGE = "Test HttpRequestException";
            public int SendCallCount { get; private set; }

            private readonly HttpStatusCode _responseCode;
            private readonly bool _throwHttpRequestException;

            public MockHttpClientHandler(bool throwHttpRequestException)
            {
                _throwHttpRequestException = throwHttpRequestException;
            }

            public MockHttpClientHandler(HttpStatusCode responseCode)
            {
                _responseCode = responseCode;
            }

            protected override Task<HttpResponseMessage> SendAsync(
                HttpRequestMessage request,
                CancellationToken cancellationToken)
            {
                SendCallCount++;

                if (_throwHttpRequestException)
                    throw new HttpRequestException(MESSAGE);

                return Task.FromResult(new HttpResponseMessage(_responseCode));
            }
        }

        [Fact]
        public async Task Test_RetryHttpClientHandler_RetryOnHttpException()
        {
            // arrange
            var retryCount = 3;
            var mockHttpClientHandler = new MockHttpClientHandler(throwHttpRequestException: true);
            var retryHttpClientHandler = new RetryHttpClientHandler(mockHttpClientHandler)
            {
                RetryCount = retryCount,
                RetryOnHttpRequestException = true,
                FirstRetryDelay = TimeSpan.FromMilliseconds(100)
            };
            var httpClient = new HttpClient(retryHttpClientHandler);
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "http://localhost");
            HttpRequestException? exception = null;

            // act
            try
            {
                await httpClient.SendAsync(httpRequestMessage);
            }
            catch (HttpRequestException e)
            {
                exception = e;
            }

            // asserts
            Assert.Equal(retryCount, mockHttpClientHandler.SendCallCount - 1);
            Assert.NotNull(exception);
            Assert.Equal(MockHttpClientHandler.MESSAGE, exception.Message);
        }

        [Fact]
        public async Task Test_RetryHttpClientHandler_RetryOnServerError()
        {
            // arrange
            var retryCount = 3;
            var mockHttpClientHandler = new MockHttpClientHandler(HttpStatusCode.InternalServerError);
            var retryHttpClientHandler = new RetryHttpClientHandler(mockHttpClientHandler)
            {
                RetryCount = retryCount,
                RetryOnHttpRequestException = true,
                FirstRetryDelay = TimeSpan.FromMilliseconds(100)
            };
            var httpClient = new HttpClient(retryHttpClientHandler);
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "http://localhost");

            // act
            var response = await httpClient.SendAsync(httpRequestMessage);

            // asserts
            Assert.Equal(retryCount, mockHttpClientHandler.SendCallCount - 1);
            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        [Fact]
        public async Task Test_RetryHttpClientHandler_RetryOnRequestTimeoutError()
        {
            // arrange
            var retryCount = 3;
            var mockHttpClientHandler = new MockHttpClientHandler(HttpStatusCode.RequestTimeout);
            var retryHttpClientHandler = new RetryHttpClientHandler(mockHttpClientHandler)
            {
                RetryCount = retryCount,
                RetryOnHttpRequestException = true,
                FirstRetryDelay = TimeSpan.FromMilliseconds(100)
            };
            var httpClient = new HttpClient(retryHttpClientHandler);
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "http://localhost");

            // act
            var response = await httpClient.SendAsync(httpRequestMessage);

            // asserts
            Assert.Equal(retryCount, mockHttpClientHandler.SendCallCount - 1);
            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.RequestTimeout, response.StatusCode);
        }

        [Fact]
        public async Task Test_RetryHttpClientHandler_RetryOnTooManyRequestsError()
        {
            // arrange
            var retryCount = 3;
            var mockHttpClientHandler = new MockHttpClientHandler(HttpStatusCode.TooManyRequests);
            var retryHttpClientHandler = new RetryHttpClientHandler(mockHttpClientHandler)
            {
                RetryCount = retryCount,
                RetryOnHttpRequestException = true,
                FirstRetryDelay = TimeSpan.FromMilliseconds(100)
            };
            var httpClient = new HttpClient(retryHttpClientHandler);
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "http://localhost");

            // act
            var response = await httpClient.SendAsync(httpRequestMessage);

            // asserts
            Assert.Equal(retryCount, mockHttpClientHandler.SendCallCount - 1);
            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);
        }

        [Fact]
        public async Task Test_RetryHttpClientHandler_NoRetryOnBadRequestError()
        {
            // arrange
            var retryCount = 3;
            var mockHttpClientHandler = new MockHttpClientHandler(HttpStatusCode.BadRequest);
            var retryHttpClientHandler = new RetryHttpClientHandler(mockHttpClientHandler)
            {
                RetryCount = retryCount,
                RetryOnHttpRequestException = true,
                FirstRetryDelay = TimeSpan.FromMilliseconds(100)
            };
            var httpClient = new HttpClient(retryHttpClientHandler);
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "http://localhost");

            // act
            var response = await httpClient.SendAsync(httpRequestMessage);

            // asserts
            Assert.Equal(1, mockHttpClientHandler.SendCallCount);
            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Test_RetryHttpClientHandler_NoRetryOnSuccess()
        {
            // arrange
            var retryCount = 3;
            var mockHttpClientHandler = new MockHttpClientHandler(HttpStatusCode.OK);
            var retryHttpClientHandler = new RetryHttpClientHandler(mockHttpClientHandler)
            {
                RetryCount = retryCount,
                RetryOnHttpRequestException = true,
                FirstRetryDelay = TimeSpan.FromMilliseconds(100)
            };
            var httpClient = new HttpClient(retryHttpClientHandler);
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "http://localhost");

            // act
            var response = await httpClient.SendAsync(httpRequestMessage);

            // asserts
            Assert.Equal(1, mockHttpClientHandler.SendCallCount);
            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Test_RetryHttpClientHandler_RateLimit()
        {
            // arrange
            var rateLimit = 5;
            var rateLimitPeriod = TimeSpan.FromSeconds(1);
            var retryCount = 3;
            var totalRequests = 15;
            var expectedDuration = rateLimitPeriod * (totalRequests - rateLimit) / rateLimit;

            var mockHttpClientHandler = new MockHttpClientHandler(HttpStatusCode.OK);
            var retryHttpClientHandler = new RetryHttpClientHandler(mockHttpClientHandler)
            {
                RetryCount = retryCount,
                RetryOnHttpRequestException = true,
                FirstRetryDelay = TimeSpan.FromMilliseconds(100),
                RateGate = new RateGate(
                    occurrences: rateLimit,
                    timeUnit: rateLimitPeriod)
            };
            var httpClient = new HttpClient(retryHttpClientHandler);

            var responses = new HttpResponseMessage[totalRequests];
            var stopWatch = Stopwatch.StartNew();

            // act
            for (var i = 0; i < totalRequests; ++i)
            {
                var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "http://localhost");
                responses[i] = await httpClient.SendAsync(httpRequestMessage);
            }

            stopWatch.Stop();
 
            // asserts
            Assert.Equal(totalRequests, mockHttpClientHandler.SendCallCount);
            Assert.True(stopWatch.Elapsed >= expectedDuration);

            foreach (var response in responses)
            {
                Assert.NotNull(response);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }
    }
}
