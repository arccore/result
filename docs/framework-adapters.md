# ASP.NET Core Adapters

`Arccore.Result.AspNetCore` contains optional adapters for ASP.NET Core applications.

The adapter package depends on `Arccore.Result`, while the core package remains independent from ASP.NET Core.

## Minimal API

Use `ToMinimalApiResult` from `Arccore.Result.AspNetCore.MinimalApi`:

```csharp
using Arccore.Result;
using Arccore.Result.AspNetCore.MinimalApi;

app.MapGet("/users/{id:guid}", (Guid id) =>
{
    Result<UserDto> result = Result<UserDto>.Success(
        new UserDto(id, "Ada"),
        "User loaded.");

    return result.ToMinimalApiResult();
});
```

For `Created` and `Accepted` results, pass an optional location URI:

```csharp
app.MapPost("/users", () =>
{
    Result<UserDto> result = Result<UserDto>.Created(
        new UserDto(Guid.NewGuid(), "Ada"),
        "User created.");

    return result.ToMinimalApiResult($"/users/{result.Value!.Id}");
});
```

## MVC

Use `ToActionResult` from `Arccore.Result.AspNetCore.Mvc`:

```csharp
using Arccore.Result;
using Arccore.Result.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("users")]
public sealed class UsersController : ControllerBase
{
    [HttpGet("{id:guid}")]
    public IActionResult Get(Guid id)
    {
        Result<UserDto> result = Result<UserDto>.Success(
            new UserDto(id, "Ada"),
            "User loaded.");

        return result.ToActionResult();
    }
}
```

For `Created` and `Accepted` results, pass an optional location URI:

```csharp
[HttpPost]
public IActionResult Create()
{
    Result<UserDto> result = Result<UserDto>.Created(
        new UserDto(Guid.NewGuid(), "Ada"),
        "User created.");

    return result.ToActionResult($"/users/{result.Value!.Id}");
}
```

## Success Mapping

| ResultType | Minimal API | MVC |
| --- | --- | --- |
| `Success` | `200 OK` | `OkObjectResult` |
| `Created` | `201 Created` | `ObjectResult` with `201` |
| `Accepted` | `202 Accepted` | `ObjectResult` with `202` |
| `NoContent` | `204 No Content` | `NoContentResult` |

For `Result<TValue>`, success responses return the value payload except for `NoContent`.

`Created` and `Accepted` mappings include a `Location` header when a location URI is passed to the adapter method.

For message-only `Result`, success responses return:

```json
{
  "message": "Saved."
}
```

## Failure Mapping

Failures are returned as `ProblemDetails`.

| ResultType | Status |
| --- | --- |
| `Validation` | `400 Bad Request` |
| `NotFound` | `404 Not Found` |
| `Conflict` | `409 Conflict` |
| `Unauthorized` | `401 Unauthorized` |
| `Forbidden` | `403 Forbidden` |
| `BusinessRule` | `422 Unprocessable Entity` |
| `Unexpected` | `500 Internal Server Error` |

Single-error failures include an `errorCode` extension:

```json
{
  "title": "Resource not found",
  "status": 404,
  "detail": "user.not_found: User was not found.",
  "errorCode": "user.not_found"
}
```

Multi-error failures include an `errors` extension:

```json
{
  "title": "Validation failed",
  "status": 400,
  "detail": "Validation failed.",
  "errors": [
    {
      "code": "name.required",
      "message": "Name is required."
    }
  ]
}
```
