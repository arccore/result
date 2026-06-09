using Arccore.Result.Constants;
using Arccore.Result.Exceptions;
using Arccore.Result.Manager;

namespace Arccore.Result.Extenstions;

/// <summary>
/// Provides validation helpers for <see cref="ResultType"/> values.
/// </summary>
public static class ResultTypeExtensions
{
    public static void ValidateSuccessResult(this ResultType type)
    {
        if (type is not (
            ResultType.Success
            or ResultType.Created
            or ResultType.NoContent))
        {
            throw new ResultException($"{ResultMessages.SuccessResultException} ResultType: {type}");
        }
    }

    public static void ValidateFailureResult(this ResultType type)
    {
        if (type is not (
            ResultType.Forbidden
            or ResultType.Validation
            or ResultType.NotFound
            or ResultType.BusinessRule
            or ResultType.Conflict
            or ResultType.Unexpected))
        {
            throw new ResultException($"{ResultMessages.FailureResultException} ResultType: {type}");
        }
    }
}
