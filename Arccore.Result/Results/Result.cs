using Arccore.Result.Constants;
using Arccore.Result.Extensions;
using Arccore.Result.Validation;

namespace Arccore.Result;

/// <summary>
/// Represents an operation result that does not carry a payload.
/// </summary>
public sealed class Result : IResult
{
    private Result(string userMessage, ResultType type)
    {
        type.EnsureSuccessType();

        IsSuccess = true;
        Type = type;
        UserMessage = ResultGuard.UserMessage(userMessage);
    }

    private Result(ResultFailure failure, ResultType type, string? userMessage)
    {
        type.EnsureFailureType();

        IsSuccess = false;
        Type = type;
        Failure = failure;
        UserMessage = CreateFailureUserMessage(failure, userMessage);
    }

    /// <inheritdoc />
    public bool IsSuccess { get; }

    /// <inheritdoc />
    public ResultType Type { get; }

    /// <inheritdoc />
    public string UserMessage { get; }

    /// <inheritdoc />
    public ResultFailure? Failure { get; }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    public static Result Success(string message)
        => new(message, ResultType.Success);

    /// <summary>
    /// Creates a successful created result.
    /// </summary>
    public static Result Created(string message)
        => new(message, ResultType.Created);

    /// <summary>
    /// Creates a successful accepted result.
    /// </summary>
    public static Result Accepted(string message)
        => new(message, ResultType.Accepted);

    /// <summary>
    /// Creates a successful no-content result.
    /// </summary>
    public static Result NoContent(string message)
        => new(message, ResultType.NoContent);

    /// <summary>
    /// Creates a validation failure result.
    /// </summary>
    public static Result Validation(Error error, string? message = null)
        => FailureResult(ResultFailure.Single(error), ResultType.Validation, message);

    /// <summary>
    /// Creates a validation failure result with multiple errors.
    /// </summary>
    public static Result Validation(IEnumerable<Error> errors, string message)
        => FailureResult(ResultFailure.Many(errors), ResultType.Validation, ResultGuard.UserMessage(message));

    /// <summary>
    /// Creates a not-found failure result.
    /// </summary>
    public static Result NotFound(Error error, string? message = null)
        => FailureResult(ResultFailure.Single(error), ResultType.NotFound, message);

    /// <summary>
    /// Creates a conflict failure result.
    /// </summary>
    public static Result Conflict(Error error, string? message = null)
        => FailureResult(ResultFailure.Single(error), ResultType.Conflict, message);

    /// <summary>
    /// Creates an unauthorized failure result.
    /// </summary>
    public static Result Unauthorized(Error error, string? message = null)
        => FailureResult(ResultFailure.Single(error), ResultType.Unauthorized, message);

    /// <summary>
    /// Creates a forbidden failure result.
    /// </summary>
    public static Result Forbidden(Error error, string? message = null)
        => FailureResult(ResultFailure.Single(error), ResultType.Forbidden, message);

    /// <summary>
    /// Creates a business-rule failure result.
    /// </summary>
    public static Result BusinessRule(Error error, string? message = null)
        => FailureResult(ResultFailure.Single(error), ResultType.BusinessRule, message);

    /// <summary>
    /// Creates an unexpected failure result.
    /// </summary>
    public static Result Unexpected(Error error, string? message = null)
        => FailureResult(ResultFailure.Single(error), ResultType.Unexpected, message);

    /// <summary>
    /// Creates an unexpected failure result with multiple errors.
    /// </summary>
    public static Result Unexpected(IEnumerable<Error> errors, string message)
        => FailureResult(ResultFailure.Many(errors), ResultType.Unexpected, ResultGuard.UserMessage(message));

    private static Result FailureResult(ResultFailure failure, ResultType type, string? message)
        => new(failure, type, message);

    private static string CreateFailureUserMessage(ResultFailure failure, string? message)
    {
        if (!string.IsNullOrWhiteSpace(message))
        {
            return message;
        }

        return failure.IsList ? message! : failure.Error!.ToString();
    }
}
