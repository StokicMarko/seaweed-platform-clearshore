using Entities;

namespace Services;

/// Upload rules per media type. (Filled from the "MediaUpload" section of appsettings.json, so limits can change without touching code.)
public class MediaUploadOptions
{
    public const string SectionName = "MediaUpload";

    public Dictionary<MediaType, MediaFileRule> Rules { get; set; } = new();
    
    /// The biggest limit of all media types. The server must accept at least this many bytes ;p
    public long LargestFileBytes => Rules.Values.Select(rule => rule.MaxBytes).DefaultIfEmpty(0).Max();
}