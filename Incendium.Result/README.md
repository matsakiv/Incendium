# Incendium.Result
[![License: MIT](https://img.shields.io/github/license/matsakiv/incendium)](https://opensource.org/licenses/MIT) ![NuGet Version](https://img.shields.io/nuget/v/Incendium.Result) ![NuGet Downloads](https://img.shields.io/nuget/dt/Incendium.Result)

Incendium.Result is a small .NET Standard 2.1 library, which provides `Error`, `Result<T>` and `NullableResult<T>` useful types.

These types allow you to return success value or error from asynchronous and synchronous methods without explicit indication of the result type when returning and with convenient type deconstruction during processing the result.

These type also can be used for less error handling through exception mechanisms where possible.

## Getting started

### Installation

`PM> Install-Package Incendium.Result`

### Result`<T>`

If you need to return either a not-null value or an error from a method, you can use the `Result<T>` type:

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

The `Result<T>` instance can be created only from non-null value or from non-null error:

```cs
public Result<Foo> GetFooAsync() {
    return new Foo(); // correct
    return new Error(); // correct
    return (Foo)null; // incorrect, CS8600 warning, throws ArgumentNullException
    return (Foo)null!; // incorrect, throws ArgumentNullException
    return (Foo?)null; // incorrect, CS8604 warning, throws ArgumentNullException
    return (Foo?)null!; // incorrect, throws ArgumentNullException
    return (Error?)null; // incorrect, throws ArgumentNullException
}
```

##  NullableResult`<T>`

If the successful return value can be null, you must use the `NullableResult<T>` type:

```cs
public async NullableResult<Foo> GetFooAsync() {
    return new Foo(); // correct
    return new Error(); // correct
    return (Foo?)null; // correct
    return (Foo)null; // correct with CS8600 warning
    return (Foo)null!; // correct
    return (Error?)null; // incorrect, throws ArgumentNullException
}
```
