namespace Arccore.Result;

/// <summary>
/// Represents the canonical outcome of an operation.
/// </summary>
public interface IResult
{
    /// <summary>
    /// Gets a value indicating whether the operation completed successfully.
    /// </summary>
    bool IsSuccess { get; }

    /// <summary>
    /// Gets the classified result status.
    /// </summary>
    ResultType Type { get; }

    /// <summary>
    /// Gets the user-facing message for the operation outcome.
    /// </summary>
    string UserMessage { get; }

    /// <summary>
    /// Gets the failure details when the operation failed; otherwise, <see langword="null"/>.
    /// </summary>
    ResultFailure? Failure { get; }
}
