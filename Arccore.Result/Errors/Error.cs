namespace Arccore.Result.Errors;

/// <summary>
/// Defines a domain error with a code and a descriptive message.
/// </summary>
public record Error(string Code, string Message)
{
    public override string ToString() => $"{Code}: {Message}";
}
