namespace Arccore.Result.AspNetCore.Models;

/// <summary>
/// Represents a message-only success response for results without a payload.
/// </summary>
/// <param name="Message">The user-facing result message.</param>
public sealed record ResultResponse(string Message);
