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

    public List<string> Validate(
        string? fileName,
        long fileSize,
        IReadOnlyList<MediaUploadRuleDto> rules,
        IEnumerable<string> knownTypes,
        bool requireFile = true)
    {
        List<string> errors = new();

        // Add requires a file. Edit may retain the existing file.
        if (string.IsNullOrWhiteSpace(fileName))
        {
            if (requireFile)
            {
                errors.Add("Choose a picture, video or 3D model.");
            }
        }
        else
        {
            ValidateFile(fileName, fileSize, rules, errors);
        }

        // At least one available technology must be selected.
        if (TechnologyTypes.Count == 0)
        {
            errors.Add("Select at least one technology.");
        }

        HashSet<string> known = knownTypes.ToHashSet();

        if (TechnologyTypes.Any(type => !known.Contains(type)))
        {
            errors.Add(
                "One or more technologies are no longer available. " +
                "Reload the options and select again.");
        }

        Required(TitleEn, "English title", errors);
        Required(TitleEs, "Spanish title", errors);
        Required(TitleFr, "French title", errors);

        // Descriptions are optional, but must be complete if supplied.
        bool hasDescription =
            !string.IsNullOrWhiteSpace(DescriptionEn) ||
            !string.IsNullOrWhiteSpace(DescriptionEs) ||
            !string.IsNullOrWhiteSpace(DescriptionFr);

        if (hasDescription)
        {
            Required(DescriptionEn, "English description", errors);
            Required(DescriptionEs, "Spanish description", errors);
            Required(DescriptionFr, "French description", errors);
        }

        return errors;
    }

    private static void ValidateFile(
        string fileName,
        long fileSize,
        IReadOnlyList<MediaUploadRuleDto> rules,
        List<string> errors)
    {
        if (fileSize <= 0)
        {
            errors.Add("The selected file is empty.");
        }

        if (rules.Count == 0)
        {
            errors.Add(
                "Upload settings are unavailable. " +
                "Load them before submitting.");

            return;
        }

        MediaUploadRuleDto? rule = FindRule(fileName, rules);

        if (rule is null)
        {
            errors.Add(
                "Unsupported file format. Choose one of the formats " +
                "listed above the file picker.");

            return;
        }

        if (fileSize > rule.MaxBytes)
        {
            errors.Add(
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
        string field,
        List<string> errors)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            errors.Add($"Enter the {field}.");
        }
    }
}