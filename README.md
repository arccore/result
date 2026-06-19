# Arccore.Result

`Arccore.Result` is a small result model for returning operation outcomes without mixing success data, failure details, and exception-based control flow.

## Current Shape

- `Result` represents an operation without a payload.
- `Result<TValue>` represents an operation with a payload.
- `IResult` exposes the shared result contract.
- `Error` represents a stable error code and human-readable message.
- `ResultFailure` stores either one error or a materialized list of errors.
- `ResultType` classifies the result as success, created, accepted, no content, validation, not found, conflict, unauthorized, forbidden, business rule, or unexpected.
- `Arccore.Result.AspNetCore` provides optional Minimal API and MVC adapters.

## Example

```csharp
using Arccore.Result;

public Result<int> FindCount(string name)
{
    if (string.IsNullOrWhiteSpace(name))
    {
        return Result<int>.Validation(
            new Error("name.required", "Name is required."));
    }

    return Result<int>.Success(42, "Count loaded.");
}
```

## Build And Test

```powershell
dotnet build Arccore.Result.slnx
dotnet run --project Arccore.Result.Tests\Arccore.Result.Tests.csproj
dotnet run --project Arccore.Result.AspNetCore.Tests\Arccore.Result.AspNetCore.Tests.csproj
```

## Documentation

- [Core results](docs/result.md)
- [Extensions](docs/extensions.md)
- [ASP.NET Core adapters](docs/framework-adapters.md)

