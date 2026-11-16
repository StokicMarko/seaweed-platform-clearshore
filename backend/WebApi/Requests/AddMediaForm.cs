namespace WebApi.Requests;

/// The multipart form sent by the dashboard (Flat fields, a form cannot send nested objects)
/// Everything is nullable (on purpose), missing values are reported by the service as validation errors (user gets one list of problems instead of two different error formats)
public class AddMediaForm
{
    public IFormFile? File { get; set; }
    public List<string> TechnologyCodes { get; set; } = [];

    public string? TitleEn { get; set; }
    public string? TitleEs { get; set; }
    public string? TitleFr { get; set; }

    public string? DescriptionEn { get; set; }
    public string? DescriptionEs { get; set; }
    public string? DescriptionFr { get; set; }
}