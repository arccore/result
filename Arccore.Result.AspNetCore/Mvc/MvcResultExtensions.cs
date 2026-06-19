using Arccore.Result.AspNetCore.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Arccore.Result.AspNetCore.Mvc;

/// <summary>
/// Provides MVC adapters for <see cref="Result"/> and <see cref="Result{TValue}"/>.
/// </summary>
public static class MvcResultExtensions
{
    /// <summary>
    /// Converts a result without a payload to an MVC action result.
    /// </summary>
    /// <param name="result">The operation result.</param>
    /// <returns>An action result mapped from the result type.</returns>
    public static IActionResult ToActionResult(this Result result)
        => result.ToActionResult(location: null);

    /// <summary>
    /// Converts a result without a payload to an MVC action result.
    /// </summary>
    /// <param name="result">The operation result.</param>
    /// <param name="location">The optional location URI used by created and accepted responses.</param>
    /// <returns>An action result mapped from the result type.</returns>
    public static IActionResult ToActionResult(this Result result, string? location)
    {
        if (!result.IsSuccess)
        {
            return ToProblemObjectResult(result);
        }

        return result.Type switch
        {
            ResultType.NoContent => new NoContentResult(),
            ResultType.Created => new CreatedResult(location, ResultHttpMapper.CreateMessageResponse(result)),
            ResultType.Accepted => new AcceptedResult(location, ResultHttpMapper.CreateMessageResponse(result)),
            _ => new OkObjectResult(ResultHttpMapper.CreateMessageResponse(result))
        };
    }

    /// <summary>
    /// Converts a result with a payload to an MVC action result.
    /// </summary>
    /// <typeparam name="TValue">The payload type.</typeparam>
    /// <param name="result">The operation result.</param>
    /// <returns>An action result mapped from the result type.</returns>
    public static IActionResult ToActionResult<TValue>(this Result<TValue> result)
        => result.ToActionResult(location: null);

    /// <summary>
    /// Converts a result with a payload to an MVC action result.
    /// </summary>
    /// <typeparam name="TValue">The payload type.</typeparam>
    /// <param name="result">The operation result.</param>
    /// <param name="location">The optional location URI used by created and accepted responses.</param>
    /// <returns>An action result mapped from the result type.</returns>
    public static IActionResult ToActionResult<TValue>(this Result<TValue> result, string? location)
    {
        if (!result.IsSuccess)
        {
            return ToProblemObjectResult(result);
        }

        return result.Type switch
        {
            ResultType.NoContent => new NoContentResult(),
            ResultType.Created => new CreatedResult(location, result.Value),
            ResultType.Accepted => new AcceptedResult(location, result.Value),
            _ => new OkObjectResult(result.Value)
        };
    }

    private static ObjectResult ToProblemObjectResult(IResult result)
        => new(ResultHttpMapper.ToProblemDetails(result))
        {
            StatusCode = ResultHttpMapper.ToStatusCode(result.Type)
        };
}
