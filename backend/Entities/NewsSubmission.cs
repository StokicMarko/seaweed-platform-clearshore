namespace Entities;

/// The proposed data of a news change request, stored as its JSON payload.
public class NewsSubmission
{
    public required LocalizedText Title { get; set; }
    public required LocalizedText Text { get; set; }
    public List<MediaFile> Files { get; set; } = [];
}