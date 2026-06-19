# Core Results

`Arccore.Result` models operation outcomes with explicit success and failure states.

## Result Or Result<TValue>

Use `Result` when the operation only needs to report an outcome:

```csharp
using Arccore.Result;

public Result DeleteUser(Guid id)
{
    return Result.NoContent("User deleted.");
}
```

Use `Result<TValue>` when the operation returns data:

```csharp
using Arccore.Result;

public Result<UserDto> GetUser(Guid id)
{
    UserDto user = new(id, "Ada");

    return Result<UserDto>.Success(user, "User loaded.");
}
```

## Invariants

Successful results:

- Have `IsSuccess == true`.
- Have a success `ResultType`.
- Have `Failure == null`.
- Require a non-empty `UserMessage`.

Failure results:

- Have `IsSuccess == false`.
- Have a failure `ResultType`.
- Have `Failure != null`.
- Require at least one `Error`.

## Success Factories

`Result` supports:

```csharp
Result.Success("Saved.");
Result.Created("Created.");
Result.Accepted("Queued.");
Result.NoContent("Deleted.");
```

`Result<TValue>` supports:

```csharp
Result<int>.Success(10, "Loaded.");
Result<int>.Created(10, "Created.");
Result<int>.Accepted(10, "Queued.");
Result<int>.NoContent("No content.");
```

## Single-Error Failures

Single-error failures may omit the user message. When omitted, the result uses `Error.ToString()` as the fallback message.

```csharp
Error error = new("user.not_found", "User was not found.");

Result<UserDto> result = Result<UserDto>.NotFound(error);

// result.UserMessage == "user.not_found: User was not found."
```

Available single-error failure factories:

```csharp
Result.Validation(error);
Result.NotFound(error);
Result.Conflict(error);
Result.Unauthorized(error);
Result.Forbidden(error);
Result.BusinessRule(error);
Result.Unexpected(error);
```

The same factories are available on `Result<TValue>`.

## Multi-Error Failures

Multi-error failures require an explicit non-empty user message because no single error can safely describe the whole failure.

```csharp
Error[] errors =
[
    new("name.required", "Name is required."),
    new("email.required", "Email is required."),
];

Result result = Result.Validation(errors, "Validation failed.");
```

Currently, multi-error overloads exist for:

```csharp
Result.Validation(errors, "Validation failed.");
Result.Unexpected(errors, "Unexpected failure.");
```

The same overloads are available on `Result<TValue>`.

## Error

`Error` requires a non-empty code and message:

```csharp
Error error = new("order.conflict", "Order has already been submitted.");
```

`Error.ToString()` returns the stable code and message separated by `": "`.

## ResultFailure

`ResultFailure` represents either one error or many errors.

For a single error:

- `Error` is populated.
- `Errors` is empty.
- `IsList == false`.

For many errors:

- `Error` is `null`.
- `Errors` contains the materialized error list.
- `IsList == true`.

`ResultFailure.Many` rejects `null`, empty collections, and collections containing `null` items.

## ResultType

Success result types:

- `Success`
- `Created`
- `Accepted`
- `NoContent`

Failure result types:

- `Validation`
- `NotFound`
- `Conflict`
- `Unauthorized`
- `Forbidden`
- `BusinessRule`
- `Unexpected`

`Invalid` and unknown enum casts are not accepted by result construction paths.

