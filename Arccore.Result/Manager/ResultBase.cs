namespace Arccore.Result.Manager;

public abstract class ResultBase
{
    public bool IsSuccess { get; init; }
    public string Message { get; init; }
    public Error? Error { get; init; }
    public IEnumerable<Error>? Errors { get; init; }

    protected ResultBase(string message)
    {
        IsSuccess = true;
        Message = message;
    }

    protected ResultBase(Error error, string message = "")
    {

        if (error is null)
        {
            throw new ArgumentNullException("Can`t return failure result without error!");
        }

        IsSuccess = false;
        Message = string.IsNullOrWhiteSpace(message)? error.ToString() : message;
        Error = error;
    }

    protected ResultBase(IEnumerable<Error> errors, string message)
    {
        if (errors is null)
        {
            throw new ArgumentNullException("Can`t return failure result with error!");
        }

        IsSuccess = false;
        Message = message;
        Errors = errors;
    }

}
