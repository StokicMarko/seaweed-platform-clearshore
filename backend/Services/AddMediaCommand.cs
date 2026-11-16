using Entities;

namespace Services;

/// Everything the service needs to submit a new media item for approval (UC03).
/// Plain data with no HTTP types, so a form, a test or another caller can build it :)
public class AddMediaCommand
{
    /// The uploaded file, read as a stream so large videos are not loaded into memory (chunkss)
    public required Stream Content { get; set; }
    public required string FileName { get; set; }
    public long Length { get; set; }
    public string? ContentType { get; set; }

    /// Technology codes (filter) (the code's machine codes, check meeting). At least one.
    public List<string> TechnologyCodes { get; set; } = [];

    // Nullable on purpose - service reports missing values as validation errors ;)))) im the best im tired
    public LocalizedText? Title { get; set; }
    public LocalizedText? Description { get; set; }
}