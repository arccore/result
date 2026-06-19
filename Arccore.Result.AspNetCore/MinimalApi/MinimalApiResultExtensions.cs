using Arccore.Result.AspNetCore.Internal;
using Microsoft.AspNetCore.Http;

namespace Arccore.Result.AspNetCore.MinimalApi;

/// <summary>
/// Provides Minimal API adapters for <see cref="Result"/> and <see cref="Result{TValue}"/>.
/// </summary>
public static class MinimalApiResultExtensions
{
    /// <summary>
    /// Converts a result without a payload to a Minimal API result.
    /// </summary>
    /// <param name="result">The operation result.</param>
    /// <returns>A Minimal API result mapped from the result type.</returns>
    public static Microsoft.AspNetCore.Http.IResult ToMinimalApiResult(this Result result)
        => result.ToMinimalApiResult(location: null);

    /// <summary>
    /// Converts a result without a payload to a Minimal API result.
    /// </summary>
    /// <param name="result">The operation result.</param>
    /// <param name="location">The optional location URI used by created and accepted responses.</param>
    /// <returns>A Minimal API result mapped from the result type.</returns>
    public static Microsoft.AspNetCore.Http.IResult ToMinimalApiResult(this Result result, string? location)
    {
        if (!result.IsSuccess)
        {
            return Results.Problem(ResultHttpMapper.ToProblemDetails(result));
        }

        return result.Type switch
        {
            ResultType.NoContent => Results.NoContent(),
            ResultType.Created => Results.Created(uri: location, ResultHttpMapper.CreateMessageResponse(result)),
            ResultType.Accepted => Results.Accepted(uri: location, ResultHttpMapper.CreateMessageResponse(result)),
            _ => Results.Ok(ResultHttpMapper.CreateMessageResponse(result))
        };
    }

    /// <summary>
    /// Converts a result with a payload to a Minimal API result.
    /// </summary>
    /// <typeparam name="TValue">The payload type.</typeparam>
    /// <param name="result">The operation result.</param>
    /// <returns>A Minimal API result mapped from the result type.</returns>
    public static Microsoft.AspNetCore.Http.IResult ToMinimalApiResult<TValue>(this Result<TValue> result)
        => result.ToMinimalApiResult(location: null);

    /// <summary>
    /// Converts a result with a payload to a Minimal API result.
    /// </summary>
    /// <typeparam name="TValue">The payload type.</typeparam>
    /// <param name="result">The operation result.</param>
    /// <param name="location">The optional location URI used by created and accepted responses.</param>
    /// <returns>A Minimal API result mapped from the result type.</returns>
    public static Microsoft.AspNetCore.Http.IResult ToMinimalApiResult<TValue>(this Result<TValue> result, string? location)
    {
        if (!result.IsSuccess)
        {
            return Results.Problem(ResultHttpMapper.ToProblemDetails(result));
        }

        return result.Type switch
        {
            ResultType.NoContent => Results.NoContent(),
            ResultType.Created => Results.Created(uri: location, result.Value),
            ResultType.Accepted => Results.Accepted(uri: location, result.Value),
            _ => Results.Ok(result.Value)
        };
    }
}
