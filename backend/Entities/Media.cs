namespace Entities;

public class Media
{
    public int Id { get; set; }
    public Machine? Machine { get; set; }
    public MediaType MediaType { get; set; }
    public required LocalizedText Title { get; set; }
    public LocalizedText? Description { get; set; }
    public required string OriginalFileName { get; set; }
    public required string ContentType { get; set; }
    public DateTimeOffset PublishedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}
