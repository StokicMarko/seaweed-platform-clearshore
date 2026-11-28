namespace ApiContracts.News;

/// A published news item.
public class NewsDto
{
    public int Id { get; set; }
    public required LocalizedTextDto Title { get; set; }
    public required LocalizedTextDto Text { get; set; }
    public List<NewsFileDto> Files { get; set; } = [];
    public DateTimeOffset PublishedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}