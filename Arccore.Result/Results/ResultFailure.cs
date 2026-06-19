using Arccore.Result.Constants;

namespace Arccore.Result;

/// <summary>
/// Represents one or more errors associated with a failed result.
/// </summary>
public sealed class ResultFailure
{
    private ResultFailure(Error error)
    {
        Error = error;
        Errors = Array.Empty<Error>();
        IsList = false;
    }

    private ResultFailure(IReadOnlyList<Error> errors)
    {
        Error = null;
        Errors = errors;
        IsList = true;
    }

    /// <summary>
    /// Gets the single error for the failure, or <see langword="null"/> when the failure contains multiple errors.
    /// </summary>
    public Error? Error { get; }

    /// <summary>
    /// Gets the errors for a multi-error failure, or an empty list when the failure contains a single error.
    /// </summary>
    public IReadOnlyList<Error> Errors { get; }

    /// <summary>
    /// Gets a value indicating whether the failure was created from multiple errors.
    /// </summary>
    public bool IsList { get; }

    /// <summary>
    /// Creates a failure from a single error.
    /// </summary>
    /// <param name="error">The failure error.</param>
    /// <returns>A failure containing the provided error.</returns>
    /// <exception cref="ResultException">Thrown when <paramref name="error"/> is <see langword="null"/>.</exception>
    public static ResultFailure Single(Error error)
    {
        if (error is null)
        {
            throw new ResultException(ResultMessages.NullError);
        }

        return new ResultFailure(error);
    }

    /// <summary>
    /// Creates a failure from multiple errors.
    /// </summary>
    /// <param name="errors">The failure errors.</param>
    /// <returns>A failure containing all provided errors.</returns>
    /// <exception cref="ResultException">Thrown when <paramref name="errors"/> is <see langword="null"/> or empty.</exception>
    public static ResultFailure Many(IEnumerable<Error> errors)
    {
        if (errors is null)
        {
            throw new ResultException(ResultMessages.NullError);
        }

        Error[] materializedErrors = errors.ToArray();
        if (materializedErrors.Length == 0)
        {
            throw new ResultException(ResultMessages.EmptyErrors);
        }

        if (materializedErrors.Any(error => error is null))
        {
            throw new ResultException(ResultMessages.NullError);
        }

        return new ResultFailure(materializedErrors);
    }
}
