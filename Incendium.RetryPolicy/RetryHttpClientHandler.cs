using Incendium.RetryPolicy.Abstract;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Incendium.RetryPolicy
{
    /// <summary>
    /// Represents HTTP client handler which allows you to resend requests in case of errors and control the number of requests sent per unit of time
    /// </summary>
    /// <param name="httpMessageHandler">Inner HTTP message handler used to send requests</param>
    public class RetryHttpClientHandler(HttpMessageHandler httpMessageHandler)
        : DelegatingHandler(httpMessageHandler)
    {
        private bool _isDisposed;
        private IRateGate? _rateGate;

        /// <summary>
        /// Retry count
        /// </summary>
        public int RetryCount { get; init; }
        /// <summary>
        /// Flag indicating whether the request should be resent in case of an HttpRequestException error
        /// </summary>
        public bool RetryOnHttpRequestException { get; init; }
        /// <summary>
        /// Delay for first retry attempt
        /// </summary>
        public TimeSpan FirstRetryDelay { get; init; }
        /// <summary>
        /// An object that controls the number of requests per unit of time. If equal to null, no control is performed
        /// </summary>
        public IRateGate? RateGate { get => _rateGate; init => _rateGate = value; }
        /// <summary>
        /// A custom factory used to get a set of delays. By default Delays.DecorrelatedJitterBackoffV2 is used
        /// </summary>
        public Func<IEnumerable<TimeSpan>>? RetryDelaysFactory { get; init; }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            IEnumerator<TimeSpan>? delays = null;
            var retryCount = RetryCount;

            while (true)
            {
                try
                {
                    if (_rateGate != null)
                        await _rateGate.WaitToProceedAsync(cancellationToken);

                    var response = await base.SendAsync(request, cancellationToken);

                    if (!response.IsSuccessStatusCode &&
                        retryCount > 0 &&
                        (IsServerError(response.StatusCode) ||
                         IsRequestTimeoutError(response.StatusCode) ||
                         IsTooManyRequestsError(response.StatusCode)))
                    {
                        delays ??= GetRetryDelays().GetEnumerator();

                        if (delays.MoveNext())
                        {
                            var delay = delays.Current;

                            retryCount--;

                            await Task.Delay(delay, cancellationToken);

                            continue;
                        }
                    }

                    return response;
                }
                catch (HttpRequestException)
                {
                    if (!RetryOnHttpRequestException)
                        throw;

                    delays ??= GetRetryDelays().GetEnumerator();

                    if (retryCount > 0 && delays.MoveNext())
                    {
                        var delay = delays.Current;

                        retryCount--;

                        await Task.Delay(delay, cancellationToken);

                        continue;
                    }

                    throw;
                }
            }
        }

        private IEnumerable<TimeSpan> GetRetryDelays() => RetryDelaysFactory?.Invoke() ??
            Delays.DecorrelatedJitterBackoffV2(FirstRetryDelay, RetryCount);

        /// <summary>
        /// Check if HTTP status code represents server error (5xx) 
        /// </summary>
        /// <param name="code">HTTP status code</param>
        /// <returns>true if code represents server error, otherwise false</returns>
        public static bool IsServerError(HttpStatusCode code) =>
            code >= HttpStatusCode.InternalServerError && (int)code < 600;

        /// <summary>
        /// Check if HTTP status code represents request timeout error (408)
        /// </summary>
        /// <param name="code">HTTP status code</param>
        /// <returns>true if code represents request timeout error, otherwise false</returns>
        public static bool IsRequestTimeoutError(HttpStatusCode code) =>
            code == HttpStatusCode.RequestTimeout;

        /// <summary>
        /// Check if HTTP status code represents too many requests (429)
        /// </summary>
        /// <param name="code">HTTP status code</param>
        /// <returns>true if code represents too many request error, otherwise false</returns>
        public static bool IsTooManyRequestsError(HttpStatusCode code) =>
            code == HttpStatusCode.TooManyRequests;

        protected override void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    _rateGate?.Dispose();
                    _rateGate = null;
                }

                _isDisposed = true;
            }

            base.Dispose(disposing);
        }
    }
}
