namespace Arccore.Result.Manager;

public class Result : ResultBase
{
    private Result(string message)
        : base(message) { }

    private Result(Error error, string message = "")
        : base(error, message) { }

    private Result(IEnumerable<Error> errors, string message)
        : base(errors, message) { }

    public static Result Success(string message = "Default Success")
        => new(message);

    public static Result Failure(Error error, string message = "")
        => new(error, message);

    public static Result Failure(IEnumerable<Error> erros, string message)
        => new(erros, message);
}

public class Result<TValue> : ResultBase
{
    public TValue? Value { get; init; }

    private Result(TValue value, string message)
        : base(message)
    {
        Value = value;
    }

    private Result(Error error, string message = "")
        : base(error, message) { }

    private Result(IEnumerable<Error> error, string message)
        : base(error, message) { }

    public static Result<TValue> Success(TValue value, string message)
        => new(value, message);

    public static Result<TValue> Failure(Error error, string message = "")
        => new(error, message);

    public static Result<TValue> Failure(IEnumerable<Error> errors, string message)
        => new(errors, message);
}