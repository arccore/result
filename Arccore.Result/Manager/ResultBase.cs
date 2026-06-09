using Arccore.Result.Constants;
using Arccore.Result.Exceptions;
using Arccore.Result.Extenstions;

namespace Arccore.Result.Manager;

/// <summary>
/// Base type for the result hierarchy.
/// </summary>
public abstract class ResultBase
{
    public bool IsSuccess { get; init; }
    public ResultType Type { get; init; }
    public string Message { get; init; }
    public Error? Error { get; init; }
    public IEnumerable<Error>? Errors { get; init; }

    protected ResultBase(string message, ResultType type = ResultType.Success)
    {
        type.ValidateSuccessResult();

        IsSuccess = true;
        Type = type;
        Message = string.IsNullOrWhiteSpace(message)
            ? ResultMessages.DefaultSuccess
            : message;
    }

    protected ResultBase(Error error, ResultType type, string message = "")
    {
        type.ValidateFailureResult();

        if (error is null)
        {
            throw new ResultException(ResultMessages.NullError);
        }

        IsSuccess = false;
        Type = type;
        Error = error;
        Message = string.IsNullOrWhiteSpace(message)
            ? error.ToString()
            : message;
    }

    protected ResultBase(IEnumerable<Error> errors, string message, ResultType type)
    {
        type.ValidateFailureResult();

        if (errors is null)
        {
            throw new ResultException(ResultMessages.NullError);
        }

        IsSuccess = false;
        Type = type;
        Errors = errors;
        Message = string.IsNullOrWhiteSpace(message)
            ? ResultMessages.DefaultError
            : message;
    }
}

