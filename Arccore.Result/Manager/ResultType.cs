namespace Arccore.Result.Manager;

/// <summary>
/// Represents the status of an operation result.
/// </summary>
public enum ResultType
{
    Success,
    Created,
    NoContent,
    Forbidden,
    Validation,
    NotFound,
    BusinessRule,
    Conflict,
    Unexpected
}

