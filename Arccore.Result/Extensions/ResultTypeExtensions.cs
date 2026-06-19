using Arccore.Result.Constants;

namespace Arccore.Result.Extensions;

internal static class ResultTypeExtensions
{
    public static bool IsSuccessType(this ResultType type)
        => type is ResultType.Success
            or ResultType.Created
            or ResultType.Accepted
            or ResultType.NoContent;

    public static bool IsFailureType(this ResultType type)
        => type is ResultType.Validation
            or ResultType.NotFound
            or ResultType.Conflict
            or ResultType.Unauthorized
            or ResultType.Forbidden
            or ResultType.BusinessRule
            or ResultType.Unexpected;

    public static void EnsureSuccessType(this ResultType type)
    {
        if (!type.IsSuccessType())
        {
            throw new ResultException($"{ResultMessages.SuccessResultException}{ResultMessages.InvalidResultTypePrefix}{type}");
        }
    }

    public static void EnsureFailureType(this ResultType type)
    {
        if (!type.IsFailureType())
        {
            throw new ResultException($"{ResultMessages.FailureResultException}{ResultMessages.InvalidResultTypePrefix}{type}");
        }
    }
}
