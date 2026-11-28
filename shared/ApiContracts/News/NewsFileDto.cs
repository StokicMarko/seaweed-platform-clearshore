namespace ApiContracts.News;

/// One file of a published news item.
public class NewsFileDto
{
    public required string FileName { get; set; }
    public required string ContentType { get; set; }
    public required string FileUrl { get; set; }
}