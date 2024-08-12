# Incendium
[![License: MIT](https://img.shields.io/github/license/matsakiv/incendium)](https://opensource.org/licenses/MIT)

Incendium is a small set of useful cross-platform .NET standard 2.1 libraries for .NET developers.

| Package | Description | Nuget |
| --------- | ----------- | ----- |
| Incendium.Result | Contains `Error`, `Result<T>` and `NullableResult<T>` types which allows to return a success value or an error (something vaguely similar to the Rust style) | <a href="https://www.nuget.org/packages/Incendium.Result/"><img src="https://img.shields.io/nuget/v/Incendium.Result"/></a><a href="https://www.nuget.org/packages/Incendium.Result/"><img src="https://img.shields.io/nuget/dt/Incendium.Result"/></a> |
| Incendium.RetryPolicy | Provides `RetryHttpClientHandler` to easily retry HTTP requests in case of errors, as well as `RateGate` to control rate limiting | <a href="https://www.nuget.org/packages/Incendium.RetryPolicy/"><img src="https://img.shields.io/nuget/v/Incendium.RetryPolicy"/></a><a href="https://www.nuget.org/packages/Incendium.RetryPolicy/"><img src="https://img.shields.io/nuget/dt/Incendium.RetryPolicy"/></a> |

## Getting started

### Installation

`PM> Install-Package Incendium.Result`
`PM> Install-Package Incendium.RetryPolicy`

### `Result<T>` and `NullableResult<T>`

The method can be synchronous or asynchronous and can return a value without explicitly creating a `Result<T>` type:

```cs
public async Result<string> GetStringAsync() {
    // ...
    if (condition1) {
        return "Test string result";
    } else {
        return new Error(code: 123, message: "Test error");
    }

    try {
        // ...
    } catch (Exception e) {
        return new Error(code: 321, message: "Test error", exception: e);
    }
}
```
Then processing the result might look like this:

```cs
var (str, error) = await GetStringAsync();

if (error != null) {
    log.LogError(
        error.Exception(),
        "Error with code {@code} and message {@message}",
        error.Code(),
        error.Message());
}
```
### `RetryHttpClientHandler`

Сan be used as a handler for `HttpClient` and allows you to easily (compared to Polly) set up repeated requests in the following cases:
* `HttpNetworkException`
* Server errors (`5xx`)
* Request timeout error (`408`)
* Too many requests (`429`)

and also allows you to configure rate limiting with exponential delays for outgoing requests using `RateGate`:

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
