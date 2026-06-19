using Arccore.Result.Constants;
using Arccore.Result.Extensions;
using Arccore.Result.Validation;

namespace Arccore.Result;

/// <summary>
/// Represents an operation result that carries a payload.
/// </summary>
/// <typeparam name="TValue">The payload type.</typeparam>
public sealed class Result<TValue> : IResult
{
    private Result(TValue? value, string userMessage, ResultType type)
    {
        type.EnsureSuccessType();

        IsSuccess = true;
        Type = type;
        Value = value;
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
    /// Gets the payload returned by the operation.
    /// </summary>
    public TValue? Value { get; }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    public static Result<TValue> Success(TValue value, string message)
        => new(value, message, ResultType.Success);

    /// <summary>
    /// Creates a successful created result.
    /// </summary>
    public static Result<TValue> Created(TValue value, string message)
        => new(value, message, ResultType.Created);

    /// <summary>
    /// Creates a successful accepted result.
    /// </summary>
    public static Result<TValue> Accepted(TValue value, string message)
        => new(value, message, ResultType.Accepted);

    /// <summary>
    /// Creates a successful no-content result.
    /// </summary>
    public static Result<TValue> NoContent(string message, TValue? value = default)
        => new(value, message, ResultType.NoContent);

    /// <summary>
    /// Creates a validation failure result.
    /// </summary>
    public static Result<TValue> Validation(Error error, string? message = null)
        => FailureResult(ResultFailure.Single(error), ResultType.Validation, message);

    /// <summary>
    /// Creates a validation failure result with multiple errors.
    /// </summary>
    public static Result<TValue> Validation(IEnumerable<Error> errors, string message)
        => FailureResult(ResultFailure.Many(errors), ResultType.Validation, ResultGuard.UserMessage(message));

    /// <summary>
    /// Creates a not-found failure result.
    /// </summary>
    public static Result<TValue> NotFound(Error error, string? message = null)
        => FailureResult(ResultFailure.Single(error), ResultType.NotFound, message);

    /// <summary>
    /// Creates a conflict failure result.
    /// </summary>
    public static Result<TValue> Conflict(Error error, string? message = null)
        => FailureResult(ResultFailure.Single(error), ResultType.Conflict, message);

    /// <summary>
    /// Creates an unauthorized failure result.
    /// </summary>
    public static Result<TValue> Unauthorized(Error error, string? message = null)
        => FailureResult(ResultFailure.Single(error), ResultType.Unauthorized, message);

    /// <summary>
    /// Creates a forbidden failure result.
    /// </summary>
    public static Result<TValue> Forbidden(Error error, string? message = null)
        => FailureResult(ResultFailure.Single(error), ResultType.Forbidden, message);

    /// <summary>
    /// Creates a business-rule failure result.
    /// </summary>
    public static Result<TValue> BusinessRule(Error error, string? message = null)
        => FailureResult(ResultFailure.Single(error), ResultType.BusinessRule, message);

    /// <summary>
    /// Creates an unexpected failure result.
    /// </summary>
    public static Result<TValue> Unexpected(Error error, string? message = null)
        => FailureResult(ResultFailure.Single(error), ResultType.Unexpected, message);

    /// <summary>
    /// Creates an unexpected failure result with multiple errors.
    /// </summary>
    public static Result<TValue> Unexpected(IEnumerable<Error> errors, string message)
        => FailureResult(ResultFailure.Many(errors), ResultType.Unexpected, ResultGuard.UserMessage(message));

    private static Result<TValue> FailureResult(ResultFailure failure, ResultType type, string? message)
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
