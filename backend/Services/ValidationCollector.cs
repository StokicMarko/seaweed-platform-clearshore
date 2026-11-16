namespace Services;

/// Collects the problems found while checking a request, so they can all be reported together.
/// Create one per use-case call (do not register it in DI), because it holds state for that call only (within one request every class that asked for a ValidationCollector would get the same list if so)
public class ValidationCollector
{
    private readonly List<string> _errors = [];

    public bool HasErrors => _errors.Count > 0;

    public void Add(string error) => _errors.Add(error);

    /// Throws ValidationFailedException with every collected problem, or does nothing when there are none.
    public void ThrowIfAny()
    {
        if (HasErrors)
        {
            throw new ValidationFailedException([.. _errors]);
        }
    }
}