namespace Arccore.Result.Constants;

internal static class ResultMessages
{
    public const string SuccessResultException = "Success result has invalid result type.";
    public const string FailureResultException = "Failure result has invalid result type.";
    public const string NullError = "Cannot return a failure result without error.";
    public const string EmptyErrors = "Cannot return a failure result without at least one error.";
    public const string EmptyUserMessage = "Result user message cannot be empty.";
    public const string EmptyErrorCode = "Error code cannot be empty.";
    public const string EmptyErrorMessage = "Error message cannot be empty.";
    public const string InvalidResultTypePrefix = " ResultType: ";
    public const string ErrorSeparator = ": ";
}
