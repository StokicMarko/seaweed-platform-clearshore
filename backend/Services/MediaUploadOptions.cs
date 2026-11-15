using Entities;

namespace Services;

/// Upload rules per media type. (Filled from the "MediaUpload" section of appsettings.json, so limits can change without touching code.)
public class MediaUploadOptions
{
    public const string SectionName = "MediaUpload";

    public Dictionary<MediaType, MediaFileRule> Rules { get; set; } = new();
}