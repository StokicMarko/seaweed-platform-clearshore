namespace Services;

/// Thrown when a request breaks a business rule (a missing field, an unsupported file...).
/// Carries every problem found, so the dashboard can show them.
public class ValidationFailedException(IReadOnlyList<string> errors) : Exception("The request is not valid.")
{
    public IReadOnlyList<string> Errors { get; } = errors;
}