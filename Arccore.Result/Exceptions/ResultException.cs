namespace Arccore.Result;

/// <summary>
/// Represents an exception thrown when a result cannot be constructed without violating its invariants.
/// </summary>
public class ResultException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ResultException"/> class.
    /// </summary>
    /// <param name="message">The exception message.</param>
    public ResultException(string message) : base(message)
    {
    }
}
