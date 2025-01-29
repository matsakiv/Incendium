# Incendium
[![License: MIT](https://img.shields.io/github/license/matsakiv/incendium)](https://opensource.org/licenses/MIT)

Incendium is a small set of useful cross-platform .NET standard 2.1 libraries for .NET developers.

| Package | Description | Nuget |
| --------- | ----------- | ----- |
| [Incendium.Result](https://github.com/matsakiv/Incendium/tree/main/Incendium.Result) | Contains `Error`, `Result<T>` and `NullableResult<T>` types which allows to return a success value or an error | ![NuGet Version](https://img.shields.io/nuget/v/Incendium.Result) ![NuGet Downloads](https://img.shields.io/nuget/dt/Incendium.Result) |
| [Incendium.RetryPolicy](https://github.com/matsakiv/Incendium/tree/main/Incendium.RetryPolicy) | Contains `RetryHttpClientHandler` to easily retry HTTP requests in case of errors, as well as `RateGate` to control rate limiting | ![NuGet Version](https://img.shields.io/nuget/v/Incendium.RetryPolicy) ![NuGet Downloads](https://img.shields.io/nuget/dt/Incendium.RetryPolicy) |

## Installation

```
PM> Install-Package Incendium.Result
PM> Install-Package Incendium.RetryPolicy
```

## Documentation

- [Incendium.Result Documentation](https://github.com/matsakiv/Incendium/tree/main/Incendium.Result)
- [Incendium.RetryPolicy Documentation](https://github.com/matsakiv/Incendium/tree/main/Incendium.RetryPolicy)
