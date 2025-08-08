using Arccore.Result.Constants;

namespace Arccore.Result.Manager;

public class Result : ResultBase
{
    private Result(string message)
        : base(message) { }

    private Result(Error error, ResultType type, string message = "")
        : base(error, type, message) { }
 
    private Result(IEnumerable<Error> errors, string message, ResultType type)
        : base(errors, message, type) { }

    public static Result Success(string message = ResultMessages.DefaultSuccess)
        => new(message);

    public static Result Failure(Error error, ResultType type, string message = "")
        => new(error, type, message);

    public static Result Failure(IEnumerable<Error> erros, string message, ResultType type)
        => new(erros, message, type);
}

public class Result<TValue> : ResultBase
{
    public TValue? Value { get; init; }

    private Result(TValue value, string message)
        : base(message)
    {
        Value = value;
    }

    private Result(Error error, ResultType type, string message = "")
        : base(error, type, message) { }

    private Result(IEnumerable<Error> error, string message, ResultType type)
        : base(error, message, type) { }

    public static Result<TValue> Success(TValue value, string message)
        => new(value, message);

    public static Result<TValue> Failure(Error error, ResultType type, string message = "")
        => new(error, type, message);

    public static Result<TValue> Failure(IEnumerable<Error> errors, string message, ResultType type)
        => new(errors, message, type);
}