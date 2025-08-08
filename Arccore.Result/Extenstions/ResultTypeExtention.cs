using Arccore.Result.Constants;
using Arccore.Result.Exceptions;
using Arccore.Result.Manager;

namespace Arccore.Result.Extenstions;

public static class ResultTypeExtention
{
    public static void ValidateSuccessResult(this ResultType type)
    {
        if(type is not (
            ResultType.Success
            or ResultType.Created
            ))
        {
            throw new ResultException($"{ResultMessages.SuccessResultException} : {nameof(type)} !");
        }

    }
    
    public static void ValidateFailureResult(this ResultType type)
    {
        if(type is not (
            ResultType.Validation
            or ResultType.NotFound
            or ResultType.BusinessRule
            or ResultType.Conflict
            or ResultType.Unexpected
            ))
        {
            throw new ResultException($"{ResultMessages.FailureResultException} : {nameof(type)} !");
        }

    }
}
