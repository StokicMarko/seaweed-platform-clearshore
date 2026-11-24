namespace WebApi.Requests;

/// Multipart form for editing media. The file is optional: no file keeps the current one.
public class UpdateMediaForm
{
    public IFormFile? File { get; set; }
    public List<string> TechnologyTypes { get; set; } = [];

    public string? TitleEn { get; set; }
    public string? TitleEs { get; set; }
    public string? TitleFr { get; set; }

    public string? DescriptionEn { get; set; }
    public string? DescriptionEs { get; set; }
    public string? DescriptionFr { get; set; }
}