using System.Text.Json;
using Arccore.Result;
using Arccore.Result.AspNetCore.MinimalApi;
using Arccore.Result.AspNetCore.Models;
using Arccore.Result.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

using CoreResult = Arccore.Result.Result;
using HttpIResult = Microsoft.AspNetCore.Http.IResult;

var tests = new (string Name, Func<Task> Test)[]
{
    ("Minimal API maps message success to 200", MinimalApiMapsMessageSuccessToOk),
    ("Minimal API maps generic success to 200", MinimalApiMapsGenericSuccessToOk),
    ("Minimal API maps no content to 204", MinimalApiMapsNoContent),
    ("Minimal API maps created to 201", MinimalApiMapsCreated),
    ("Minimal API maps created location", MinimalApiMapsCreatedLocation),
    ("Minimal API maps accepted location", MinimalApiMapsAcceptedLocation),
    ("Minimal API maps single failure to problem details", MinimalApiMapsSingleFailure),
    ("Minimal API maps multi failure to problem details", MinimalApiMapsMultiFailure),
    ("MVC maps message success to OK", MvcMapsMessageSuccessToOk),
    ("MVC maps generic created to 201", MvcMapsGenericCreated),
    ("MVC maps created location", MvcMapsCreatedLocation),
    ("MVC maps accepted location", MvcMapsAcceptedLocation),
    ("MVC maps no content to 204", MvcMapsNoContent),
    ("MVC maps single failure to problem details", MvcMapsSingleFailure),
    ("MVC maps multi failure to problem details", MvcMapsMultiFailure),
};

var failed = 0;

foreach ((string name, Func<Task> test) in tests)
{
    try
    {
        await test();
        Console.WriteLine($"PASS {name}");
    }
    catch (Exception exception)
    {
        failed++;
        Console.WriteLine($"FAIL {name}");
        Console.WriteLine($"     {exception.GetType().Name}: {exception.Message}");
    }
}

Console.WriteLine();
Console.WriteLine($"{tests.Length - failed} passed, {failed} failed.");

return failed == 0 ? 0 : 1;

static async Task MinimalApiMapsMessageSuccessToOk()
{
    HttpResultExecution execution = await ExecuteAsync(CoreResult.Success("Saved.").ToMinimalApiResult());

    AssertEqual(StatusCodes.Status200OK, execution.StatusCode);
    using JsonDocument document = JsonDocument.Parse(execution.Body);
    AssertEqual("Saved.", document.RootElement.GetProperty("message").GetString());
}

static async Task MinimalApiMapsGenericSuccessToOk()
{
    HttpResultExecution execution = await ExecuteAsync(Result<int>.Success(42, "Loaded.").ToMinimalApiResult());

    AssertEqual(StatusCodes.Status200OK, execution.StatusCode);
    AssertEqual("42", execution.Body);
}

static async Task MinimalApiMapsNoContent()
{
    HttpResultExecution execution = await ExecuteAsync(CoreResult.NoContent("Deleted.").ToMinimalApiResult());

    AssertEqual(StatusCodes.Status204NoContent, execution.StatusCode);
    AssertEqual(string.Empty, execution.Body);
}

static async Task MinimalApiMapsCreated()
{
    HttpResultExecution execution = await ExecuteAsync(Result<int>.Created(42, "Created.").ToMinimalApiResult());

    AssertEqual(StatusCodes.Status201Created, execution.StatusCode);
    AssertEqual("42", execution.Body);
}

static async Task MinimalApiMapsCreatedLocation()
{
    HttpResultExecution execution = await ExecuteAsync(
        Result<int>.Created(42, "Created.").ToMinimalApiResult("/users/42"));

    AssertEqual(StatusCodes.Status201Created, execution.StatusCode);
    AssertEqual("/users/42", execution.Headers["Location"]);
    AssertEqual("42", execution.Body);
}

static async Task MinimalApiMapsAcceptedLocation()
{
    HttpResultExecution execution = await ExecuteAsync(
        Result<int>.Accepted(42, "Accepted.").ToMinimalApiResult("/operations/42"));

    AssertEqual(StatusCodes.Status202Accepted, execution.StatusCode);
    AssertEqual("/operations/42", execution.Headers["Location"]);
    AssertEqual("42", execution.Body);
}

static async Task MinimalApiMapsSingleFailure()
{
    Error error = new("user.not_found", "User was not found.");
    HttpResultExecution execution = await ExecuteAsync(Result<int>.NotFound(error).ToMinimalApiResult());

    AssertEqual(StatusCodes.Status404NotFound, execution.StatusCode);

    using JsonDocument document = JsonDocument.Parse(execution.Body);
    JsonElement root = document.RootElement;

    AssertEqual("Resource not found", root.GetProperty("title").GetString());
    AssertEqual(StatusCodes.Status404NotFound, root.GetProperty("status").GetInt32());
    AssertEqual(error.ToString(), root.GetProperty("detail").GetString());
    AssertEqual(error.Code, root.GetProperty("errorCode").GetString());
}

static async Task MinimalApiMapsMultiFailure()
{
    Error[] errors =
    [
        new("name.required", "Name is required."),
        new("email.required", "Email is required."),
    ];

    HttpResultExecution execution = await ExecuteAsync(CoreResult.Validation(errors, "Validation failed.").ToMinimalApiResult());

    AssertEqual(StatusCodes.Status400BadRequest, execution.StatusCode);

    using JsonDocument document = JsonDocument.Parse(execution.Body);
    JsonElement root = document.RootElement;
    JsonElement errorItems = root.GetProperty("errors");

    AssertEqual("Validation failed", root.GetProperty("title").GetString());
    AssertEqual("Validation failed.", root.GetProperty("detail").GetString());
    AssertEqual(2, errorItems.GetArrayLength());
    AssertEqual("name.required", errorItems[0].GetProperty("code").GetString());
}

static Task MvcMapsMessageSuccessToOk()
{
    IActionResult actionResult = CoreResult.Success("Saved.").ToActionResult();
    OkObjectResult ok = AssertIs<OkObjectResult>(actionResult);
    ResultResponse response = AssertIs<ResultResponse>(ok.Value);

    AssertEqual(StatusCodes.Status200OK, ok.StatusCode);
    AssertEqual("Saved.", response.Message);

    return Task.CompletedTask;
}

static Task MvcMapsGenericCreated()
{
    IActionResult actionResult = Result<int>.Created(42, "Created.").ToActionResult();
    CreatedResult created = AssertIs<CreatedResult>(actionResult);

    AssertEqual(StatusCodes.Status201Created, created.StatusCode);
    AssertEqual(42, created.Value);

    return Task.CompletedTask;
}

static Task MvcMapsCreatedLocation()
{
    IActionResult actionResult = Result<int>.Created(42, "Created.").ToActionResult("/users/42");
    CreatedResult created = AssertIs<CreatedResult>(actionResult);

    AssertEqual(StatusCodes.Status201Created, created.StatusCode);
    AssertEqual("/users/42", created.Location);
    AssertEqual(42, created.Value);

    return Task.CompletedTask;
}

static Task MvcMapsAcceptedLocation()
{
    IActionResult actionResult = Result<int>.Accepted(42, "Accepted.").ToActionResult("/operations/42");
    AcceptedResult accepted = AssertIs<AcceptedResult>(actionResult);

    AssertEqual(StatusCodes.Status202Accepted, accepted.StatusCode);
    AssertEqual("/operations/42", accepted.Location);
    AssertEqual(42, accepted.Value);

    return Task.CompletedTask;
}

static Task MvcMapsNoContent()
{
    IActionResult actionResult = CoreResult.NoContent("Deleted.").ToActionResult();
    NoContentResult noContent = AssertIs<NoContentResult>(actionResult);

    AssertEqual(StatusCodes.Status204NoContent, noContent.StatusCode);

    return Task.CompletedTask;
}

static Task MvcMapsSingleFailure()
{
    Error error = new("user.not_found", "User was not found.");
    IActionResult actionResult = Result<int>.NotFound(error).ToActionResult();
    ObjectResult objectResult = AssertIs<ObjectResult>(actionResult);
    ProblemDetails problemDetails = AssertIs<ProblemDetails>(objectResult.Value);

    AssertEqual(StatusCodes.Status404NotFound, objectResult.StatusCode);
    AssertEqual("Resource not found", problemDetails.Title);
    AssertEqual(StatusCodes.Status404NotFound, problemDetails.Status);
    AssertEqual(error.ToString(), problemDetails.Detail);
    AssertEqual(error.Code, problemDetails.Extensions["errorCode"]);

    return Task.CompletedTask;
}

static Task MvcMapsMultiFailure()
{
    Error[] errors =
    [
        new("name.required", "Name is required."),
        new("email.required", "Email is required."),
    ];

    IActionResult actionResult = CoreResult.Validation(errors, "Validation failed.").ToActionResult();
    ObjectResult objectResult = AssertIs<ObjectResult>(actionResult);
    ProblemDetails problemDetails = AssertIs<ProblemDetails>(objectResult.Value);
    ErrorDetail[] errorDetails = AssertIs<ErrorDetail[]>(problemDetails.Extensions["errors"]);

    AssertEqual(StatusCodes.Status400BadRequest, objectResult.StatusCode);
    AssertEqual("Validation failed", problemDetails.Title);
    AssertEqual(StatusCodes.Status400BadRequest, problemDetails.Status);
    AssertEqual("Validation failed.", problemDetails.Detail);
    AssertEqual(2, errorDetails.Length);
    AssertEqual("name.required", errorDetails[0].Code);

    return Task.CompletedTask;
}

static async Task<HttpResultExecution> ExecuteAsync(HttpIResult result)
{
    DefaultHttpContext httpContext = new();
    httpContext.RequestServices = new ServiceCollection()
        .AddLogging()
        .BuildServiceProvider();

    await using MemoryStream body = new();
    httpContext.Response.Body = body;

    await result.ExecuteAsync(httpContext);

    body.Position = 0;
    using StreamReader reader = new(body);
    string responseBody = await reader.ReadToEndAsync();

    Dictionary<string, string> headers = httpContext.Response.Headers.ToDictionary(
        header => header.Key,
        header => header.Value.ToString());

    return new HttpResultExecution(httpContext.Response.StatusCode, responseBody, headers);
}

static T AssertIs<T>(object? value)
{
    if (value is T typed)
    {
        return typed;
    }

    throw new InvalidOperationException($"Expected {typeof(T).Name} but got {value?.GetType().Name ?? "null"}.");
}

static void AssertEqual<T>(T expected, T actual)
{
    if (!EqualityComparer<T>.Default.Equals(expected, actual))
    {
        throw new InvalidOperationException($"Expected '{expected}' but got '{actual}'.");
    }
}

internal sealed record HttpResultExecution(int StatusCode, string Body, IReadOnlyDictionary<string, string> Headers);
