namespace Entities;

/// A published news item.
public class NewsItem
{
    public int Id { get; set; }
    public required LocalizedText Title { get; set; }
    public required LocalizedText Text { get; set; }
    public List<MediaFile> Files { get; set; } = [];
    public DateTimeOffset PublishedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}