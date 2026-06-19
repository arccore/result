using Arccore.Result.AspNetCore.Constants;
using Arccore.Result.AspNetCore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Arccore.Result.AspNetCore.Internal;

internal static class ResultHttpMapper
{
    public static int ToStatusCode(ResultType type)
        => type switch
        {
            ResultType.Success => StatusCodes.Status200OK,
            ResultType.Created => StatusCodes.Status201Created,
            ResultType.Accepted => StatusCodes.Status202Accepted,
            ResultType.NoContent => StatusCodes.Status204NoContent,
            ResultType.Validation => StatusCodes.Status400BadRequest,
            ResultType.NotFound => StatusCodes.Status404NotFound,
            ResultType.Conflict => StatusCodes.Status409Conflict,
            ResultType.Unauthorized => StatusCodes.Status401Unauthorized,
            ResultType.Forbidden => StatusCodes.Status403Forbidden,
            ResultType.BusinessRule => StatusCodes.Status422UnprocessableEntity,
            ResultType.Unexpected => StatusCodes.Status500InternalServerError,
            _ => StatusCodes.Status500InternalServerError
        };

    public static string ToTitle(ResultType type)
        => type switch
        {
            ResultType.Validation => ProblemDetailsTitles.Validation,
            ResultType.NotFound => ProblemDetailsTitles.NotFound,
            ResultType.Conflict => ProblemDetailsTitles.Conflict,
            ResultType.Unauthorized => ProblemDetailsTitles.Unauthorized,
            ResultType.Forbidden => ProblemDetailsTitles.Forbidden,
            ResultType.BusinessRule => ProblemDetailsTitles.BusinessRule,
            ResultType.Unexpected => ProblemDetailsTitles.Unexpected,
            _ => ProblemDetailsTitles.Failure
        };

    public static ProblemDetails ToProblemDetails(IResult result)
    {
        ProblemDetails problemDetails = new()
        {
            Title = ToTitle(result.Type),
            Detail = result.UserMessage,
            Status = ToStatusCode(result.Type)
        };

        AddFailureExtensions(problemDetails, result.Failure);

        return problemDetails;
    }

    public static object CreateMessageResponse(IResult result)
        => new ResultResponse(result.UserMessage);

    private static void AddFailureExtensions(ProblemDetails problemDetails, ResultFailure? failure)
    {
        if (failure is null)
        {
            return;
        }

        if (failure.IsList)
        {
            problemDetails.Extensions[ProblemDetailsKeys.Errors] = failure.Errors
                .Select(error => new ErrorDetail(error.Code, error.Message))
                .ToArray();

            return;
        }

        if (failure.Error is not null)
        {
            problemDetails.Extensions[ProblemDetailsKeys.ErrorCode] = failure.Error.Code;
        }
    }
}
