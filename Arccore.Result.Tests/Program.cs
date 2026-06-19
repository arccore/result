using Arccore.Result;
using Arccore.Result.Constants;
using Arccore.Result.Extensions;

using CoreResult = Arccore.Result.Result;

var tests = new (string Name, Action Test)[]
{
    ("Success creates successful result", SuccessCreatesSuccessfulResult),
    ("Created creates successful result", CreatedCreatesSuccessfulResult),
    ("Accepted creates successful result", AcceptedCreatesSuccessfulResult),
    ("NoContent creates successful result", NoContentCreatesSuccessfulResult),
    ("Generic success carries value", GenericSuccessCarriesValue),
    ("Generic failure has no value", GenericFailureHasNoValue),
    ("Single failure falls back to error text", SingleFailureFallsBackToErrorText),
    ("Multi validation failure requires message", MultiValidationFailureRequiresMessage),
    ("Multi unexpected failure requires message", MultiUnexpectedFailureRequiresMessage),
    ("ResultFailure rejects null single error", ResultFailureRejectsNullSingleError),
    ("ResultFailure rejects null error list", ResultFailureRejectsNullErrorList),
    ("ResultFailure rejects empty error list", ResultFailureRejectsEmptyErrorList),
    ("ResultFailure rejects null item", ResultFailureRejectsNullItem),
    ("Error rejects empty code", ErrorRejectsEmptyCode),
    ("Error rejects empty message", ErrorRejectsEmptyMessage),
    ("ResultType helpers classify known values", ResultTypeHelpersClassifyKnownValues),
    ("ResultType helpers reject invalid casts", ResultTypeHelpersRejectInvalidCasts),
};

var failed = 0;

foreach ((string name, Action test) in tests)
{
    try
    {
        test();
        Console.WriteLine($"PASS {name}");
    }
    catch (Exception exception)
    {
        failed++;
        Console.WriteLine($"FAIL {name}");
        Console.WriteLine($"     {exception.GetType().Name}: {exception.Message}");
    }
}

Console.WriteLine();
Console.WriteLine($"{tests.Length - failed} passed, {failed} failed.");

return failed == 0 ? 0 : 1;

static void SuccessCreatesSuccessfulResult()
{
    CoreResult result = CoreResult.Success("Saved.");

    AssertTrue(result.IsSuccess);
    AssertEqual(ResultType.Success, result.Type);
    AssertEqual("Saved.", result.UserMessage);
    AssertNull(result.Failure);
}

static void CreatedCreatesSuccessfulResult()
{
    CoreResult result = CoreResult.Created("Created.");

    AssertTrue(result.IsSuccess);
    AssertEqual(ResultType.Created, result.Type);
    AssertEqual("Created.", result.UserMessage);
    AssertNull(result.Failure);
}

static void AcceptedCreatesSuccessfulResult()
{
    CoreResult result = CoreResult.Accepted("Accepted.");

    AssertTrue(result.IsSuccess);
    AssertEqual(ResultType.Accepted, result.Type);
    AssertEqual("Accepted.", result.UserMessage);
    AssertNull(result.Failure);
}

static void NoContentCreatesSuccessfulResult()
{
    CoreResult result = CoreResult.NoContent("No content.");

    AssertTrue(result.IsSuccess);
    AssertEqual(ResultType.NoContent, result.Type);
    AssertEqual("No content.", result.UserMessage);
    AssertNull(result.Failure);
}

static void GenericSuccessCarriesValue()
{
    Result<int> result = Result<int>.Success(42, "Loaded.");

    AssertTrue(result.IsSuccess);
    AssertEqual(ResultType.Success, result.Type);
    AssertEqual("Loaded.", result.UserMessage);
    AssertEqual(42, result.Value);
    AssertNull(result.Failure);
}

static void GenericFailureHasNoValue()
{
    Error error = new("missing", "Item was not found.");
    Result<int> result = Result<int>.NotFound(error);

    AssertFalse(result.IsSuccess);
    AssertEqual(ResultType.NotFound, result.Type);
    AssertEqual(default, result.Value);
    AssertNotNull(result.Failure);
}

static void SingleFailureFallsBackToErrorText()
{
    Error error = new("validation.name", "Name is required.");
    CoreResult result = CoreResult.Validation(error);

    AssertFalse(result.IsSuccess);
    AssertEqual(ResultType.Validation, result.Type);
    AssertEqual(error.ToString(), result.UserMessage);
    AssertSame(error, result.Failure!.Error);
    AssertFalse(result.Failure.IsList);
}

static void MultiValidationFailureRequiresMessage()
{
    Error[] errors =
    [
        new("validation.name", "Name is required."),
        new("validation.email", "Email is required."),
    ];

    CoreResult result = CoreResult.Validation(errors, "Validation failed.");

    AssertFalse(result.IsSuccess);
    AssertEqual(ResultType.Validation, result.Type);
    AssertEqual("Validation failed.", result.UserMessage);
    AssertTrue(result.Failure!.IsList);
    AssertEqual(2, result.Failure.Errors.Count);

    AssertThrows<ResultException>(() => CoreResult.Validation(errors, ""));
}

static void MultiUnexpectedFailureRequiresMessage()
{
    Error[] errors =
    [
        new("unexpected.io", "The file could not be read."),
        new("unexpected.retry", "The retry failed."),
    ];

    Result<string> result = Result<string>.Unexpected(errors, "Unexpected failure.");

    AssertFalse(result.IsSuccess);
    AssertEqual(ResultType.Unexpected, result.Type);
    AssertEqual("Unexpected failure.", result.UserMessage);
    AssertTrue(result.Failure!.IsList);
    AssertEqual(2, result.Failure.Errors.Count);

    AssertThrows<ResultException>(() => Result<string>.Unexpected(errors, " "));
}

static void ResultFailureRejectsNullSingleError()
{
    AssertThrows<ResultException>(() => ResultFailure.Single(null!));
}

static void ResultFailureRejectsNullErrorList()
{
    AssertThrows<ResultException>(() => ResultFailure.Many(null!));
}

static void ResultFailureRejectsEmptyErrorList()
{
    AssertThrows<ResultException>(() => ResultFailure.Many([]));
}

static void ResultFailureRejectsNullItem()
{
    Error[] errors = [null!];

    AssertThrows<ResultException>(() => ResultFailure.Many(errors));
}

static void ErrorRejectsEmptyCode()
{
    ResultException exception = AssertThrows<ResultException>(() => new Error("", "Message."));

    AssertEqual(ResultMessages.EmptyErrorCode, exception.Message);
}

static void ErrorRejectsEmptyMessage()
{
    ResultException exception = AssertThrows<ResultException>(() => new Error("code", " "));

    AssertEqual(ResultMessages.EmptyErrorMessage, exception.Message);
}

static void ResultTypeHelpersClassifyKnownValues()
{
    AssertTrue(ResultType.Success.IsSuccessType());
    AssertTrue(ResultType.Created.IsSuccessType());
    AssertTrue(ResultType.Accepted.IsSuccessType());
    AssertTrue(ResultType.NoContent.IsSuccessType());

    AssertTrue(ResultType.Validation.IsFailureType());
    AssertTrue(ResultType.NotFound.IsFailureType());
    AssertTrue(ResultType.Conflict.IsFailureType());
    AssertTrue(ResultType.Unauthorized.IsFailureType());
    AssertTrue(ResultType.Forbidden.IsFailureType());
    AssertTrue(ResultType.BusinessRule.IsFailureType());
    AssertTrue(ResultType.Unexpected.IsFailureType());
}

static void ResultTypeHelpersRejectInvalidCasts()
{
    ResultType unknown = (ResultType)999;

    AssertFalse(ResultType.Invalid.IsSuccessType());
    AssertFalse(ResultType.Invalid.IsFailureType());
    AssertFalse(unknown.IsSuccessType());
    AssertFalse(unknown.IsFailureType());

    AssertThrows<ResultException>(() => ResultType.Invalid.EnsureSuccessType());
    AssertThrows<ResultException>(() => ResultType.Invalid.EnsureFailureType());
    AssertThrows<ResultException>(() => unknown.EnsureSuccessType());
    AssertThrows<ResultException>(() => unknown.EnsureFailureType());
}

static void AssertTrue(bool value)
{
    if (!value)
    {
        throw new InvalidOperationException("Expected true.");
    }
}

static void AssertFalse(bool value)
{
    if (value)
    {
        throw new InvalidOperationException("Expected false.");
    }
}

static void AssertNull(object? value)
{
    if (value is not null)
    {
        throw new InvalidOperationException($"Expected null but got '{value}'.");
    }
}

static void AssertNotNull(object? value)
{
    if (value is null)
    {
        throw new InvalidOperationException("Expected a value but got null.");
    }
}

static void AssertSame(object? expected, object? actual)
{
    if (!ReferenceEquals(expected, actual))
    {
        throw new InvalidOperationException($"Expected same instance but got '{actual}'.");
    }
}

static void AssertEqual<T>(T expected, T actual)
{
    if (!EqualityComparer<T>.Default.Equals(expected, actual))
    {
        throw new InvalidOperationException($"Expected '{expected}' but got '{actual}'.");
    }
}

static TException AssertThrows<TException>(Action action)
    where TException : Exception
{
    try
    {
        action();
    }
    catch (TException exception)
    {
        return exception;
    }
    catch (Exception exception)
    {
        throw new InvalidOperationException(
            $"Expected {typeof(TException).Name} but got {exception.GetType().Name}.",
            exception);
    }

    throw new InvalidOperationException($"Expected {typeof(TException).Name}.");
}
