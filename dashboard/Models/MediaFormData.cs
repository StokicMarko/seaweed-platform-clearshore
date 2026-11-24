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
        IEnumerable<string> knownTypes)
    {
        Dictionary<string, string> errors = new();

        // Check the file before uploading.
        if (string.IsNullOrWhiteSpace(fileName))
        {
            errors["file"] = "Choose a picture, video or 3D model.";
        }
        else
        {
            MediaUploadRuleDto? rule = FindRule(fileName, rules);

            if (fileSize <= 0)
            {
                errors.TryAdd("file", "The selected file is empty.");
            }

            if (rules.Count == 0)
            {
                errors.TryAdd(
                    "file",
                    "The form is not ready yet. Use Try again above before submitting.");
            }
            else if (rule is null)
            {
                errors.TryAdd(
                    "file",
                    "This file type is not supported. Choose one of the file types listed above.");
            }
            else if (fileSize > rule.MaxBytes)
            {
                errors.TryAdd(
                    "file",
                    $"The selected file is too large. The limit for {rule.MediaType} is {rule.MaxBytes / 1048576d:0.##} MiB.");
            }
        }

        // At least one technology must be selected.
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

        // All titles and descriptions are required.
        Required(TitleEn, "title-en", "Enter the English title.", errors);
        Required(TitleEs, "title-es", "Enter the Spanish title.", errors);
        Required(TitleFr, "title-fr", "Enter the French title.", errors);

        Required(DescriptionEn, "description-en", "Enter the English description.", errors);
        Required(DescriptionEs, "description-es", "Enter the Spanish description.", errors);
        Required(DescriptionFr, "description-fr", "Enter the French description.", errors);

        return errors;
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
        string value,
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