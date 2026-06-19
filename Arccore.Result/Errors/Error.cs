using Arccore.Result.Constants;
using Arccore.Result.Validation;

namespace Arccore.Result;

/// <summary>
/// Defines a domain error with a code and a descriptive message.
/// </summary>
public sealed record Error
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Error"/> record.
    /// </summary>
    /// <param name="code">The stable error code.</param>
    /// <param name="message">The human-readable error message.</param>
    /// <exception cref="ResultException">Thrown when <paramref name="code"/> or <paramref name="message"/> is empty.</exception>
    public Error(string code, string message)
    {
        Code = ResultGuard.ErrorCode(code);
        Message = ResultGuard.ErrorMessage(message);
    }

    /// <summary>
    /// Gets the stable error code.
    /// </summary>
    public string Code { get; }

    /// <summary>
    /// Gets the human-readable error message.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Returns the error in a readable code and message format.
    /// </summary>
    /// <returns>The formatted error.</returns>
    public override string ToString() => $"{Code}{ResultMessages.ErrorSeparator}{Message}";
}
