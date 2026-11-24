using Entities;

namespace Services;

/// Everything needed to submit an edit of a media item (UC04). The file is optional: no file means the current one is kept.
public class UpdateMediaCommand
{
    public Stream? Content { get; set; }
    public string? FileName { get; set; }
    public long Length { get; set; }
    public string? ContentType { get; set; }

    public List<string> TechnologyTypes { get; set; } = [];
    public LocalizedText? Title { get; set; }
    public LocalizedText? Description { get; set; }
}