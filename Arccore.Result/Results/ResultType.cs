namespace Arccore.Result;

/// <summary>
/// Represents the classified status of an operation result.
/// </summary>
public enum ResultType
{
    /// <summary>
    /// Represents an unset or invalid result type.
    /// </summary>
    Invalid = 0,

    /// <summary>
    /// Represents a successful operation.
    /// </summary>
    Success = 1,

    /// <summary>
    /// Represents a successful operation that created a resource.
    /// </summary>
    Created = 2,

    /// <summary>
    /// Represents a successful operation that was accepted for processing.
    /// </summary>
    Accepted = 3,

    /// <summary>
    /// Represents a successful operation with no response content.
    /// </summary>
    NoContent = 4,

    /// <summary>
    /// Represents a validation failure.
    /// </summary>
    Validation = 5,

    /// <summary>
    /// Represents a missing resource failure.
    /// </summary>
    NotFound = 6,

    /// <summary>
    /// Represents a conflicting state failure.
    /// </summary>
    Conflict = 7,

    /// <summary>
    /// Represents an authentication failure.
    /// </summary>
    Unauthorized = 8,

    /// <summary>
    /// Represents an authorization failure.
    /// </summary>
    Forbidden = 9,

    /// <summary>
    /// Represents a domain or business rule failure.
    /// </summary>
    BusinessRule = 10,

    /// <summary>
    /// Represents an unexpected failure.
    /// </summary>
    Unexpected = 11
}
