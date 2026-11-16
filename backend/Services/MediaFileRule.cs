namespace Services;

/// Which files are accepted for one media type (research)
public class MediaFileRule
{
    /// Allowed file extensions, including the dot, for example ".mp4".
    public List<string> AllowedExtensions { get; set; } = [];

    /// Largest file accepted, in bytes.
    public long MaxBytes { get; set; }
}