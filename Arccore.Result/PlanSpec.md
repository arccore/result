# Arccore.Result Implementation Plan

This document defines the planned direction for the `Arccore.Result` library. It is split into framework/tooling work, core result model work, and extension/adaptor work so each change can be implemented and reviewed independently.

## Current Status

Completed:

- Core project targets .NET 10 and builds through `Arccore.Result.slnx`.
- `.slnx` solution includes the core project and the lightweight test project.
- Core result model includes `IResult`, `Result`, `Result<TValue>`, `ResultFailure`, `ResultType`, `Error`, and `ResultException`.
- Result construction invariants are enforced by private constructors, static factories, guard helpers, and result type validation helpers.
- Multi-error failures require a non-empty user message.
- `ResultFailure.Many` rejects `null`, empty collections, and `null` error items.
- Public API XML comments exist and XML documentation file generation is enabled.
- Core invariant tests cover success, failure, invalid enum values, generic result behavior, error validation, and extension validation helpers.
- Core README and documentation pages exist for result usage and extension behavior.
- GitHub Actions CI restores, builds, and runs the lightweight test project.
- Core package has simple NuGet metadata and includes the README in packed output.

Remaining:

- Decide whether ASP.NET adapters should live in the core project or a separate package before implementation.
- Add adapter tests when Minimal API or MVC mappings are implemented.

## Goals

- Provide a small, predictable result model for operation outcomes.
- Preserve clear invariants for success and failure states.
- Keep the core package independent from ASP.NET Core unless adapter packages are intentionally added.
- Improve public API discoverability through XML documentation and examples.
- Modernize the project tooling without mixing infrastructure changes into API redesign work.
- Avoid magic strings and duplicated literal values in production code.

## Non-Goals

- Do not add HTTP framework dependencies to the core result assembly unless a separate adapter project is not feasible.
- Do not adopt new C# language features only for novelty.
- Do not perform broad refactors that are unrelated to result construction, validation, documentation, or framework adapters.
- Do not hard-code repeated strings, status labels, extension keys, media types, or default messages directly inside result, MVC, or Minimal API mapping logic.

## Constants And Standard Values Policy

Production code should not use magic strings or scattered literal values.

Rules:

- Prefer standard framework constants or APIs when they exist.
- Use project-owned constants when the value is part of this library's contract.
- Keep constants close to their domain: core result constants in the core project, ASP.NET adapter constants in the adapter project.
- Avoid public constants unless the value is intentionally part of the public API.
- Prefer internal static classes for implementation constants.
- Do not duplicate string literals across factories, validation helpers, HTTP mappers, tests, or documentation examples.
- Tests may use literals for readability, but repeated contract values should reference the same constants as production code when practical.

Core examples:

```csharp
internal static class ResultMessages
{
    public const string EmptyUserMessage = "Result user message cannot be empty.";
    public const string EmptyErrorCode = "Error code cannot be empty.";
    public const string EmptyErrorMessage = "Error message cannot be empty.";
}
```

Framework adapter examples:

```csharp
internal static class ProblemDetailsKeys
{
    public const string ErrorCode = "errorCode";
    public const string Errors = "errors";
}
```

Use standard library values for HTTP status codes where available, for example `StatusCodes.Status404NotFound` in ASP.NET Core adapters.

Acceptance criteria:

- No repeated production string literals are introduced during implementation.
- HTTP status codes use framework constants such as `StatusCodes.Status200OK`, `StatusCodes.Status400BadRequest`, and related ASP.NET Core values.
- ProblemDetails extension keys are centralized.
- Guard and exception messages are centralized.
- Analyzer warnings or code review should reject new magic strings in result construction and framework mapping code.

## Framework And Tooling Plan

### Target .NET 10

Update project and build configuration to target .NET 10.

Implementation notes:

- Change `Arccore.Result.csproj` from `net8.0` to `net10.0`.
- Add or update `global.json` only if the repository needs SDK pinning.
- Update CI/build pipelines to install and use the .NET 10 SDK.
- Keep nullable reference types and implicit usings enabled.

Acceptance criteria:

- `dotnet restore` succeeds.
- `dotnet build` succeeds on .NET 10.
- CI uses the same SDK version as local development or a compatible .NET 10 SDK range.

### Adopt C# 14 Carefully

Use C# 14 only where it improves readability, safety, or maintainability.

Implementation notes:

- Do not rewrite stable code just to use new syntax.
- Prefer established patterns already present in the library.
- If a new language feature is introduced, ensure it does not make the public API harder to consume.

Acceptance criteria:

- Any C# 14 feature used has a clear reason in the code review.
- No feature adoption changes public behavior accidentally.

### Migrate To `.slnx`

Move the solution to the `.slnx` format after the core API plan is settled.

Implementation notes:

- Keep the existing `.sln` until the `.slnx` migration is verified.
- Ensure IDE support and CI support are both confirmed.
- Treat this as a tooling-only change.

Acceptance criteria:

- The solution opens correctly in supported IDEs.
- CI can restore, build, and test from the new solution layout.

## Core Result Model Plan

### Public API Shape

Introduce a canonical `IResult` interface.

Proposed shape:

```csharp
public interface IResult
{
    bool IsSuccess { get; }
    ResultType Type { get; }
    string UserMessage { get; }
    ResultFailure? Failure { get; }
}
```

Design decisions:

- `Failure` is `null` for successful results.
- `Failure` is required for failed results.
- `UserMessage` replaces the existing `Message` concept.
- Success results require an explicit non-empty user message.
- Single-error failures may omit the user message and fall back to `Error.ToString()`.
- Multi-error failures require an explicit non-empty user message because no single error can safely represent the whole failure.

Acceptance criteria:

- Success results always have `IsSuccess == true` and `Failure == null`.
- Failure results always have `IsSuccess == false` and `Failure != null`.
- Success result messages are never generated from generic defaults.
- Multi-error failure messages are required.
- Public XML documentation explains every member and invariant.

### ResultFailure Model

Add a dedicated failure model that represents either one error or multiple errors.

Proposed shape:

```csharp
public sealed class ResultFailure
{
    public Error? Error { get; }
    public IReadOnlyList<Error> Errors { get; }
    public bool IsList { get; }
}
```

Design decisions:

- Use factory methods to prevent invalid states.
- For a single failure, `Error` is populated, `Errors` contains that same error or is empty by documented choice, and `IsList == false`.
- For multiple failures, `Errors` must contain at least one item, `Error` is either `null` or the first error by documented choice, and `IsList == true`.
- Prefer `IReadOnlyList<Error>` over `IEnumerable<Error>` so failures are materialized once and cannot change during enumeration.

Required factories:

```csharp
public static ResultFailure Single(Error error);
public static ResultFailure Many(IEnumerable<Error> errors);
```

Acceptance criteria:

- `Single(null)` throws `ResultException`.
- `Many(null)` throws `ResultException`.
- `Many(empty)` throws `ResultException`.
- The model cannot represent both success and failure at the same time.

### BaseResult Implementation

Replace the current public `ResultBase` with an internal `BaseResult`, or keep `ResultBase` public only if backward compatibility is required.

Design decisions:

- Internal construction should preserve invariants.
- Concrete result types should be created through static factories.
- Public init setters should be avoided for invariant-bearing properties.

Acceptance criteria:

- External consumers cannot construct invalid result states.
- All success and failure construction routes pass through one shared validation path.
- Existing public API compatibility is explicitly documented if `ResultBase` remains public.

### Result And Result<T>

Provide concrete result types for operations with and without a payload.

Required types:

```csharp
public sealed class Result : IResult
public sealed class Result<TValue> : IResult
```

Design decisions:

- `Result` represents an operation without a value.
- `Result<TValue>` represents an operation with a value.
- `Result<TValue>.Value` is available only for successful value results.
- Failed `Result<TValue>` instances should not require a `TValue`.

Required success factories:

```csharp
Result.Success(message)
Result.Created(message)
Result.Accepted(message)
Result.NoContent(message)

Result<TValue>.Success(TValue value, message)
Result<TValue>.Created(TValue value, message)
Result<TValue>.Accepted(TValue value, message)
Result<TValue>.NoContent(message, ...)
```

Required failure factories:

```csharp
Result.Validation(...)
Result.NotFound(...)
Result.Conflict(...)
Result.Unauthorized(...)
Result.Forbidden(...)
Result.BusinessRule(...)
Result.Unexpected(...)
```

The same failure factory set should exist on `Result<TValue>`.

Acceptance criteria:

- Each public factory has XML documentation.
- Each factory maps to exactly one `ResultType`.
- Success factories cannot accept failure data.
- Failure factories require at least one `Error`.
- Multi-error failure factories require a non-empty user message.
- Single-error failure factories may use the error as the fallback user message.

### ResultType Enum

Standardize status classification while planning compatibility with existing values.

Proposed enum:

```csharp
public enum ResultType
{
    Invalid = 0,
    Success,
    Created,
    Accepted,
    NoContent,
    Validation,
    NotFound,
    Conflict,
    Unauthorized,
    Forbidden,
    BusinessRule,
    Unexpected
}
```

Design decisions:

- Keep `Forbidden` and `BusinessRule` unless there is a deliberate breaking-change reason to remove them.
- Add `Invalid = 0` so default enum values are not valid successes.
- Add `Accepted` for HTTP 202-style workflows.
- Add `Unauthorized` separately from `Forbidden`.
- Validate all enum input because callers can cast arbitrary integers to enum values.

Acceptance criteria:

- Success validation accepts only `Success`, `Created`, `Accepted`, and `NoContent`.
- Failure validation accepts only failure result types.
- `Invalid` is never accepted by success or failure factories.
- Unit tests cover invalid enum casts.

### Factory Abstraction

Do not add `IResultFactory<TResult>` until there is a clear use case that static factories cannot satisfy.

Rationale:

- Static named factories are simple and discoverable.
- A factory keyed only by `ResultType` cannot prevent invalid enum casts by itself.
- Generic factories become awkward for `Result<TValue>` because success creation requires a value.

If a factory is later needed, it should expose typed methods rather than only a generic `Create(ResultType)` method.

Acceptance criteria:

- The first implementation keeps static factories unless a concrete integration requires dependency-injected factories.
- Any future factory still validates `ResultType` values internally.

### Error Model

Keep the `Error` model simple and immutable.

Current shape:

```csharp
public record Error(string Code, string Message);
```

Implementation notes:

- Add XML documentation for the record and parameters.
- Validate empty `Code` and `Message` inside the `Error` constructor so all errors are valid from creation.
- Avoid adding HTTP-specific fields to the core error model.

Acceptance criteria:

- Error documentation describes `Code` and `Message`.
- Empty error codes and messages throw `ResultException`.
- Error remains framework-agnostic.

### Exceptions

Keep `ResultException` for invalid result construction and invariant violations.

Implementation notes:

- Add XML documentation.
- Consider adding standard constructors only if serialization or inner exceptions are needed.
- Exception messages should be stable enough for debugging, but tests should prefer exception type over exact message unless the message is part of the contract.

Acceptance criteria:

- Invalid construction paths throw `ResultException`.
- Public documentation explains when the exception is thrown.

## Extensions Plan

### Namespace And Folder Rename

Rename `Extenstions` to `Extensions`.

Implementation notes:

- Rename the folder.
- Rename the namespace from `Arccore.Result.Extenstions` to `Arccore.Result.Extensions`.
- Rename `ResultTypeExtention.cs` to `ResultTypeExtensions.cs`.
- Update all `using` statements.

Acceptance criteria:

- The misspelled namespace is no longer used in source.
- The project builds after the rename.

### ResultType Extensions

Keep result type validation helpers internal unless consumers need them.

Recommended methods:

```csharp
internal static bool IsSuccessType(this ResultType type);
internal static bool IsFailureType(this ResultType type);
internal static void EnsureSuccessType(this ResultType type);
internal static void EnsureFailureType(this ResultType type);
```

Design decisions:

- Boolean helpers are useful for mapping and tests.
- Throwing helpers centralize invariant enforcement.
- Methods should reject `Invalid` and unknown enum casts.

Acceptance criteria:

- Validation helpers cover every enum value.
- Unknown enum casts are rejected.
- XML documentation is added only if helpers remain public.

## Framework Adapter Documentation

Framework adapters should be documented separately from the core result model.

### Minimal API Adapter

Chosen location:

- Core project folder: `Extensions/MinimalApi`
- Keep framework-specific code isolated in this folder.
- Revisit a separate `Arccore.Result.AspNetCore` project/package if ASP.NET dependencies make the core package too heavy.

Purpose:

- Convert `Result` and `Result<TValue>` to `Microsoft.AspNetCore.Http.IResult`.
- Return framework-native success responses.
- Return one `ProblemDetails` response for failures.
- Avoid magic strings by using ASP.NET Core constants where available and adapter-owned constants where the framework has no standard value.

Mapping guidance:

| ResultType | Minimal API response |
| --- | --- |
| Success | `Results.Ok(...)` |
| Created | `Results.Created(...)` |
| Accepted | `Results.Accepted(...)` |
| NoContent | `Results.NoContent()` |
| Validation | `Results.ValidationProblem(...)` or `Results.Problem(...)` |
| NotFound | `Results.Problem(statusCode: 404, ...)` |
| Conflict | `Results.Problem(statusCode: 409, ...)` |
| Unauthorized | `Results.Problem(statusCode: 401, ...)` |
| Forbidden | `Results.Problem(statusCode: 403, ...)` |
| BusinessRule | `Results.Problem(statusCode: 422, ...)` |
| Unexpected | `Results.Problem(statusCode: 500, ...)` |

Implementation notes:

- Use `StatusCodes.Status200OK`, `StatusCodes.Status201Created`, `StatusCodes.Status202Accepted`, `StatusCodes.Status204NoContent`, `StatusCodes.Status400BadRequest`, and other ASP.NET Core status constants instead of numeric literals when building responses.
- Centralize `ProblemDetails.Extensions` keys such as error code, error list, trace id, and validation metadata.
- Centralize default ProblemDetails titles for each `ResultType` if the framework does not provide a suitable standard value.
- Prefer framework helpers such as `Results.ValidationProblem(...)` when they provide the correct standard shape.
- Avoid embedding response labels like `"Validation failed"` or `"Unexpected error"` inside mapper methods.

Acceptance criteria:

- Any ASP.NET Core dependency added to the core project is explicit and isolated to framework adapter code.
- Adapter methods are documented with examples.
- Failure responses include error codes and messages.
- Tests verify status code mapping.
- Mapper code contains no duplicated string literals for ProblemDetails keys, titles, or default messages.

### MVC Adapter

Chosen location:

- Core project folder: `Extensions/Mvc`
- Keep framework-specific code isolated in this folder.
- Revisit a separate `Arccore.Result.AspNetCore` project/package if ASP.NET dependencies make the core package too heavy.

Purpose:

- Convert `Result` and `Result<TValue>` to `IActionResult`.
- Return framework-native success responses.
- Return `ObjectResult`, `ProblemDetails`, or `ValidationProblemDetails` for failures.
- Avoid magic strings by using MVC/ASP.NET Core constants where available and adapter-owned constants for library-specific metadata.

Mapping guidance:

| ResultType | MVC response |
| --- | --- |
| Success | `Ok(...)` |
| Created | `Created(...)` |
| Accepted | `Accepted(...)` |
| NoContent | `NoContent()` |
| Validation | `ValidationProblem(...)` |
| NotFound | `Problem(statusCode: 404, ...)` or `NotFound(...)` |
| Conflict | `Conflict(...)` or `Problem(statusCode: 409, ...)` |
| Unauthorized | `Unauthorized(...)` |
| Forbidden | `Forbid(...)` or `Problem(statusCode: 403, ...)` |
| BusinessRule | `Problem(statusCode: 422, ...)` |
| Unexpected | `Problem(statusCode: 500, ...)` |

Implementation notes:

- Use ASP.NET Core `StatusCodes` constants for status code assignments.
- Centralize all `ProblemDetails.Extensions` keys in the adapter project.
- Centralize default titles and fallback messages used by MVC mappers.
- Prefer MVC helper methods when they create a standard framework response shape.
- Do not hard-code validation dictionary keys, error extension keys, or fallback titles directly inside controller extension methods.

Acceptance criteria:

- MVC adapter documentation includes controller examples.
- Failure mapping is consistent with Minimal API mapping.
- Validation failures preserve field-level errors when available.
- Mapper code contains no duplicated string literals for ProblemDetails keys, titles, or default messages.

## Documentation Plan

### Core Result Documentation

Create documentation that explains:

- What a result is.
- When to use `Result` versus `Result<TValue>`.
- How success and failure invariants work.
- How to create single-error and multi-error failures.
- How `ResultType` should be interpreted.
- How the API changed from the current `Message`, `Error`, and `Errors` shape if the change is breaking.

Recommended file:

- `docs/result.md`

### Extension Documentation

Create documentation that explains:

- Available core extensions.
- Result type validation behavior.
- Which extension methods are public and which are internal implementation details.

Recommended file:

- `docs/extensions.md`

### Framework Adapter Documentation

Create documentation that explains:

- Minimal API mappings.
- MVC mappings.
- `ProblemDetails` response shape.
- Adapter constants for ProblemDetails keys, default titles, and fallback messages.
- Which standard ASP.NET Core constants are used for status codes and response behavior.
- Required NuGet packages and target framework.
- Examples for success, validation failure, not found, and unexpected failure.

Recommended file:

- `docs/framework-adapters.md`

## Testing Plan

Add focused tests for core behavior before or alongside API changes.

Required test areas:

- Success result construction.
- Failure result construction.
- Single failure and multiple failure invariants.
- Success message validation.
- Multi-error failure message validation.
- Single-error failure fallback message behavior.
- Error code and message validation.
- `ResultType.Invalid` rejection.
- Unknown enum cast rejection.
- `Result<TValue>` value handling.
- Extension validation helpers.
- ASP.NET adapter status mappings if adapters are added.

Acceptance criteria:

- Core tests do not require ASP.NET Core.
- Adapter tests live with the adapter project/package.
- Tests cover both `Result` and `Result<TValue>`.

## Suggested Implementation Order

1. Completed: Fix spelling and naming issues: `Extenstions` to `Extensions`, `ResultTypeExtention.cs` to `ResultTypeExtensions.cs`.
2. Completed: Add focused tests around core behavior so intentional breaking changes are visible.
3. Completed: Add `Invalid`, `Accepted`, and `Unauthorized` to `ResultType`; keep `Forbidden` and `BusinessRule`.
4. Completed: Introduce `ResultFailure` and centralize failure invariants.
5. Completed: Introduce `IResult` and decide whether `Message` remains as an obsolete alias for `UserMessage`.
6. Completed: Remove public `ResultBase` from the new API shape; shared construction logic stays private/internal behind concrete result factories and helpers.
7. Completed: Update `Result` and `Result<TValue>` factories.
8. Completed: Add XML documentation to public APIs and enable XML documentation output.
9. Completed: Upgrade to .NET 10 and verify local build.
10. Completed: Migrate to `.slnx` and include the test project.
11. Completed: Add core documentation files.
12. Completed: Add GitHub Actions CI restore/build/test.
13. Later: Add ASP.NET Core adapter project/package and framework adapter documentation.

## Breaking Change Checklist

Resolved decisions:

- `Message` is renamed to `UserMessage`; `Message` will not be kept as a compatibility alias.
- Public `ResultBase` is removed. Shared construction logic should stay private/internal behind concrete result factories and helper types.
- `Error` and `Errors` move behind `ResultFailure`; public results expose failure details through `Failure`.
- ASP.NET adapters currently live in the core project under separate folders: `Extensions/MinimalApi` and `Extensions/Mvc`.
- `Forbidden` and `BusinessRule` remain supported because they represent useful, distinct failure categories.

## Definition Of Done

- Completed: Core package builds on .NET 10.
- Completed: Public APIs have XML documentation and XML docs are generated.
- Completed: Core result invariants are enforced by constructors or factories.
- Completed: Unit tests cover success, failure, invalid enum values, and generic result behavior.
- Completed: Documentation exists for core results and extensions.
- Documentation exists for framework adapters once adapters are implemented.
- ASP.NET adapter dependency boundaries are explicit.
- Completed locally: CI restore/build/test commands succeed.
