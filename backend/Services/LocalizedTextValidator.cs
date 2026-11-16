using Entities;

namespace Services;

/// Checks for text must in English, Spanish and French (reuse)
public static class LocalizedTextValidator
{
    /// All three languages must be filled in.
    public static void Required(string field, LocalizedText? text, ValidationCollector validation)
    {
        if (text is null || IsMissing(text.En) || IsMissing(text.Es) || IsMissing(text.Fr))
        {
            validation.Add($"{field} is required in English, Spanish and French.");
        }
    }

    /// Optional, but if one language is given, all three must be
    public static void OptionalAllOrNone(string field, LocalizedText? text, ValidationCollector validation)
    {
        if (text is null)
        {
            return;
        }

        int filled = new[] { text.En, text.Es, text.Fr }.Count(value => !IsMissing(value));
        if (filled != 0 && filled != 3)
        {
            validation.Add($"{field} must be given in all three languages or left empty.");
        }
    }

    private static bool IsMissing(string? value) => string.IsNullOrWhiteSpace(value);
}
