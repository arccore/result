using Arccore.Result.Constants;

namespace Arccore.Result.Manager;

/// <summary>
/// Represents an operation result that does not carry a payload.
/// </summary>
public sealed class Result : ResultBase
{
    private Result(string message, ResultType type)
        : base(message, type)
    {
    }

    private Result(Error error, ResultType type, string message = "")
        : base(error, type, message)
    {
    }

    private Result(IEnumerable<Error> errors, string message, ResultType type)
        : base(errors, message, type)
    {
    }

    public static Result Success(string message = ResultMessages.DefaultSuccess)
        => new(message, ResultType.Success);

    public static Result Created(string message = ResultMessages.DefaultCreated)
        => new(message, ResultType.Created);

    public static Result NoContent(string message = ResultMessages.DefaultNoContent)
        => new(message, ResultType.NoContent);

    public static Result Forbidden(Error error, string message = "")
        => new(error, ResultType.Forbidden, message);

    public static Result Validation(Error error, string message = "")
        => new(error, ResultType.Validation, message);

    public static Result Validation(IEnumerable<Error> errors, string message = "")
        => new(errors, message, ResultType.Validation);

    public static Result NotFound(Error error, string message = "")
        => new(error, ResultType.NotFound, message);

    public static Result BusinessRule(Error error, string message = "")
        => new(error, ResultType.BusinessRule, message);

    public static Result Conflict(Error error, string message = "")
        => new(error, ResultType.Conflict, message);

    public static Result Unexpected(Error error, string message = "")
        => new(error, ResultType.Unexpected, message);

    public static Result Unexpected(IEnumerable<Error> errors, string message = "")
        => new(errors, message, ResultType.Unexpected);
}

/// <summary>
/// Represents an operation result that carries a payload.
/// </summary>
public sealed class Result<TValue> : ResultBase
{
    /// <summary>
    /// Gets the payload returned by the operation.
    /// </summary>
    public TValue? Value { get; init; }

    private Result(TValue? value, string message, ResultType type)
        : base(message, type)
    {
        Value = value;
    }

    private Result(Error error, ResultType type, string message = "")
        : base(error, type, message)
    {
    }

    private Result(IEnumerable<Error> errors, string message, ResultType type)
        : base(errors, message, type)
    {
    }

    public static Result<TValue> Success(TValue value, string message = ResultMessages.DefaultSuccess)
        => new(value, message, ResultType.Success);

    public static Result<TValue> Created(TValue value, string message = ResultMessages.DefaultCreated)
        => new(value, message, ResultType.Created);

    public static Result<TValue> NoContent(TValue? value = default, string message = ResultMessages.DefaultNoContent)
        => new(value, message, ResultType.NoContent);

    public static Result<TValue> Forbidden(Error error, string message = "")
        => new(error, ResultType.Forbidden, message);

    public static Result<TValue> Validation(Error error, string message = "")
        => new(error, ResultType.Validation, message);

    public static Result<TValue> Validation(IEnumerable<Error> errors, string message = "")
        => new(errors, message, ResultType.Validation);

    public static Result<TValue> NotFound(Error error, string message = "")
        => new(error, ResultType.NotFound, message);

    public static Result<TValue> BusinessRule(Error error, string message = "")
        => new(error, ResultType.BusinessRule, message);

    public static Result<TValue> Conflict(Error error, string message = "")
        => new(error, ResultType.Conflict, message);

    public static Result<TValue> Unexpected(Error error, string message = "")
        => new(error, ResultType.Unexpected, message);

    public static Result<TValue> Unexpected(IEnumerable<Error> errors, string message = "")
        => new(errors, message, ResultType.Unexpected);
}