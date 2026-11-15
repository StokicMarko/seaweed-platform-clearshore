using Entities;
using Microsoft.Extensions.Options;

namespace Services;

/// Checks an uploaded file against the configured rules and works out media type.
public class MediaFileValidator(IOptions<MediaUploadOptions> options)
{
    /// Finds the media type whose rule allows the file's extension, then checks the size.
    /// When the file is not accepted, a message is added to errors and null is returned.
    public MediaType? Check(string fileName, long length, List<string> errors)
    {
        string extension = Path.GetExtension(fileName).ToLowerInvariant();

        foreach ((MediaType type, MediaFileRule rule) in options.Value.Rules)
        {
            if (!rule.AllowedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
            {
                continue;
            }

            if (length <= 0)
            {
                errors.Add("The file is empty.");
                return null;
            }

            if (length > rule.MaxBytes)
            {
                errors.Add($"The {type} file is too large. The maximum is {rule.MaxBytes / (1024 * 1024)} MB.");
                return null;
            }

            return type;
        }

        string allowed = string.Join(", ", options.Value.Rules.Values.SelectMany(rule => rule.AllowedExtensions));
        string found = extension.Length == 0 ? "no extension" : $"'{extension}'";
        errors.Add($"The file has {found}. Allowed types: {allowed}.");
        return null;
    }
}