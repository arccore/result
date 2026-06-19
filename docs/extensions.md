# Extensions

The core project currently contains internal `ResultType` extension helpers in `Arccore.Result.Extensions`.

These helpers are implementation details used by result constructors and tests to keep result type validation centralized:

```csharp
type.IsSuccessType();
type.IsFailureType();
type.EnsureSuccessType();
type.EnsureFailureType();
```

Although the methods are declared `public`, the containing `ResultTypeExtensions` class is `internal`, so these methods are not part of the public consumer API.

## Success Types

The success helpers accept only:

- `ResultType.Success`
- `ResultType.Created`
- `ResultType.Accepted`
- `ResultType.NoContent`

## Failure Types

The failure helpers accept only:

- `ResultType.Validation`
- `ResultType.NotFound`
- `ResultType.Conflict`
- `ResultType.Unauthorized`
- `ResultType.Forbidden`
- `ResultType.BusinessRule`
- `ResultType.Unexpected`

## Invalid Values

`ResultType.Invalid` and unknown enum casts such as `(ResultType)999` are rejected by the throwing helpers.

This matters because C# enums can hold values outside their declared members. The library validates enum values before constructing results so external callers cannot create invalid states through casted enum values.

