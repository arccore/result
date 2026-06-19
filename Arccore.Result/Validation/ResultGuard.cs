using Arccore.Result.Constants;

namespace Arccore.Result.Validation;

internal static class ResultGuard
{
    public static string UserMessage(string message)
        => Required(message, ResultMessages.EmptyUserMessage);

    public static string ErrorCode(string code)
        => Required(code, ResultMessages.EmptyErrorCode);

    public static string ErrorMessage(string message)
        => Required(message, ResultMessages.EmptyErrorMessage);

    private static string Required(string value, string exceptionMessage)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ResultException(exceptionMessage);
        }

        return value;
    }
}
