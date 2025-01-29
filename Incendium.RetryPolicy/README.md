# Incendium.RetryPolicy
[![License: MIT](https://img.shields.io/github/license/matsakiv/incendium)](https://opensource.org/licenses/MIT) ![NuGet Version](https://img.shields.io/nuget/v/Incendium.RetryPolicy) ![NuGet Downloads](https://img.shields.io/nuget/dt/Incendium.RetryPolicy)

Incendium.RetryPolicy is a lightweight .NET standard 2.1 library that provides robust HTTP request retry functionality and rate limiting capabilities through two main components:
- `RetryHttpClientHandler`: Handles automatic retry of failed HTTP requests
- `RateGate`: Manages rate limiting for your API calls

## Installation

Using Package Manager:
```
PM> Install-Package Incendium.RetryPolicy
```

Using .NET CLI:
```
dotnet add package Incendium.RetryPolicy
```

## Usage

### RetryHttpClientHandler

The handler automatically retries requests in the following scenarios:
- Network exceptions (`HttpRequestException`)
- Server errors (5xx status codes)
- Request timeout (408)
- Rate limit exceeded (429)

Basic setup:
```cs
var handler = new RetryHttpClientHandler(new HttpClientHandler())
{
    RetryCount = 5, // sets 5 retry attempts
    RetryOnHttpRequestException = true, // sets the retry flag in case of an HttpRequestException
    FirstRetryDelay = TimeSpan.FromMilliseconds(100), // sets the median starting delay between requests
}
var client = new HttpClient(handler);
```

### Rate Limiting

Integrate rate limiting using `RateGate`:
```cs
var handler = new RetryHttpClientHandler(new HttpClientHandler())
{
    RateGate = new RateGate(
        occurrences: 10,    // Maximum requests
        timeUnit: TimeSpan.FromSeconds(60)    // Time window
    )
};
```

### Retry Delay Strategies

The library provides three built-in delay strategies:

1. **Constant Delay**
```csharp
handler.RetryDelaysFactory = () => Delays.Constant(
    delay: TimeSpan.FromSeconds(1),
    count: 5
);
```

2. **Exponential Backoff**
```csharp
handler.RetryDelaysFactory = () => Delays.Exponential(
    firstDelay: TimeSpan.FromMilliseconds(100),
    count: 5
);
```

3. **DecorrelatedJitterBackoffV2** (Default)
```csharp
handler.RetryDelaysFactory = () => Delays.DecorrelatedJitterBackoffV2(
    firstDelay: TimeSpan.FromMilliseconds(100),
    count: 5
);
```

You can also implement custom delay strategies by providing your own `IEnumerable<TimeSpan>` through the `RetryDelaysFactory` property.

## License

This project is licensed under the MIT License - see the LICENSE file for details.
