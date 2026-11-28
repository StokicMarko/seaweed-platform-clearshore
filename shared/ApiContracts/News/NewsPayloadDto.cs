namespace ApiContracts.News;

/// The proposed data inside a news change request.
///NewsSubmission is used inside the backend. The DTO is the shape shared through REST.
public class NewsPayloadDto
{
    public required LocalizedTextDto Title { get; set; }
    public required LocalizedTextDto Text { get; set; }
    public List<NewsPayloadFileDto> Files { get; set; } = [];
}