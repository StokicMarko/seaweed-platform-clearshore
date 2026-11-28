namespace ApiContracts.News;

/// One file inside the payload of a news request.
public class NewsPayloadFileDto
{
    public required string StoredFileName { get; set; }
    public required string OriginalFileName { get; set; }
    public required string ContentType { get; set; }
}