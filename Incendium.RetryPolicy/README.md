# Incendium.RetryPolicy
[![License: MIT](https://img.shields.io/github/license/matsakiv/incendium)](https://opensource.org/licenses/MIT) ![NuGet Version](https://img.shields.io/nuget/v/Incendium.RetryPolicy) ![NuGet Downloads](https://img.shields.io/nuget/dt/Incendium.RetryPolicy)

Incendium.RetryPolicy is a small .NET standard 2.1 library that provides a `RetryHttpClientHandler` type to easily retry HTTP requests in case of errors, as well as a `RateGate` to manage rate limiting.

## Getting started

### Installation

`PM> Install-Package Incendium.RetryPolicy`

### RetryHttpClientHandler

Сan be used as a handler for `HttpClient` and allows you to easily set up repeated requests with constant or exponential delays in the following cases:
* `HttpNetworkException`
* Server errors (`5xx`)
* Request timeout error (`408`)
* Too many requests (`429`)

and also allows you to configure rate limiting using `RateGate`:

```cs
var innerHttpClientHandler = new HttpClientHandler(); // can be easily mocked
var retryHttpClientHandler = new RetryHttpClientHandler(innerHttpClientHandler)
{
    RetryCount = 5, // sets 5 retry attempts
    RetryOnHttpRequestException = true, // sets the retry flag in case of an HttpRequestException
    FirstRetryDelay = TimeSpan.FromMilliseconds(100), // sets the median starting delay between requests
    RateGate = new RateGate(
        occurrences: 10, // sets rate limit to 10 request per 60 seconds
        timeUnit: TimeSpan.FromSeconds(60))
}
var httpClient = new HttpClient(retryHttpClientHandler);
```

There are several ready-made delay algorithms that can be used:
* Constant
* Exponential
* DecorrelatedJitterBackoffV2 (default)

```cs
var retryHttpClientHandler = new RetryHttpClientHandler(innerHttpClientHandler)
{
    RetryDelaysFactory = () => Delays.Exponential(
        firstDelay: TimeSpan.FromMilliseconds(100),
        count: 10)
}
```

You can also pass your own delay algorithm in the `RetryDelaysFactory` property, which should return `IEnumerable<TimeSpan>`.
