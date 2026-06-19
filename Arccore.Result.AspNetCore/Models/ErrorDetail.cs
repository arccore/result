namespace Arccore.Result.AspNetCore.Models;

/// <summary>
/// Represents an error item included in ASP.NET Core problem details responses.
/// </summary>
/// <param name="Code">The stable error code.</param>
/// <param name="Message">The human-readable error message.</param>
public sealed record ErrorDetail(string Code, string Message);
