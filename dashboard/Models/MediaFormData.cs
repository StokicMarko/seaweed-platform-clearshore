using ApiContracts.Media;

namespace Dashboard.Models;

public class MediaFormData
{
    public string TitleEn { get; set; } = "";
    public string TitleEs { get; set; } = "";
    public string TitleFr { get; set; } = "";

    public string DescriptionEn { get; set; } = "";
    public string DescriptionEs { get; set; } = "";
    public string DescriptionFr { get; set; } = "";

    public HashSet<string> TechnologyTypes { get; set; } = new();

    public Dictionary<string, string> Validate(
        string? fileName,
        long fileSize,
        IReadOnlyList<MediaUploadRuleDto> rules,
        IEnumerable<string> knownTypes,
        bool requireFile = true)
    {
        Dictionary<string, string> errors = new();

        // Add requires a file. Edit may retain the existing file.
        if (string.IsNullOrWhiteSpace(fileName))
        {
            if (requireFile)
            {
                errors["file"] = "Choose a picture, video or 3D model.";
            }
        }
        else
        {
            ValidateFile(fileName, fileSize, rules, errors);
        }

        // At least one available technology must be selected.
        if (TechnologyTypes.Count == 0)
        {
            errors["technologies"] = "Select at least one technology.";
        }

        HashSet<string> known = knownTypes.ToHashSet();

        if (TechnologyTypes.Any(type => !known.Contains(type)))
        {
            errors.TryAdd(
                "technologies",
                "One or more chosen technologies are no longer available. Use Try again above and choose again.");
        }

        Required(TitleEn, "title-en", "Enter the English title.", errors);
        Required(TitleEs, "title-es", "Enter the Spanish title.", errors);
        Required(TitleFr, "title-fr", "Enter the French title.", errors);

        // Descriptions are optional, but must be complete if supplied.
        bool hasDescription =
            !string.IsNullOrWhiteSpace(DescriptionEn) ||
            !string.IsNullOrWhiteSpace(DescriptionEs) ||
            !string.IsNullOrWhiteSpace(DescriptionFr);

        if (hasDescription)
        {
            Required(DescriptionEn, "description-en", "Enter the English description.", errors);
            Required(DescriptionEs, "description-es", "Enter the Spanish description.", errors);
            Required(DescriptionFr, "description-fr", "Enter the French description.", errors);
        }

        return errors;
    }

    private static void ValidateFile(
        string fileName,
        long fileSize,
        IReadOnlyList<MediaUploadRuleDto> rules,
        Dictionary<string, string> errors)
    {
        if (fileSize <= 0)
        {
            errors.TryAdd("file", "The selected file is empty.");
        }

        if (rules.Count == 0)
        {
            errors.TryAdd(
                "file",
                "The form is not ready yet. Use Try again above before submitting.");

            return;
        }

        MediaUploadRuleDto? rule = FindRule(fileName, rules);

        if (rule is null)
        {
            errors.TryAdd(
                "file",
                "This file type is not supported. Choose one of the file types listed above.");

            return;
        }

        if (fileSize > rule.MaxBytes)
        {
            errors.TryAdd(
                "file",
                $"The selected file is too large. " +
                $"The limit for {rule.MediaType} is " +
                $"{rule.MaxBytes / 1048576d:0.##} MiB.");
        }
    }

    public static MediaUploadRuleDto? FindRule(
        string name,
        IReadOnlyList<MediaUploadRuleDto> rules)
    {
        string extension = Path.GetExtension(name);

        return rules.FirstOrDefault(rule =>
            rule.MaxBytes > 0 &&
            rule.AllowedExtensions.Any(allowed =>
                string.Equals(
                    allowed,
                    extension,
                    StringComparison.OrdinalIgnoreCase)));
    }

    private static void Required(
        string? value,
        string key,
        string message,
        Dictionary<string, string> errors)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            errors[key] = message;
        }
    }
}