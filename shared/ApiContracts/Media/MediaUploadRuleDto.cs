namespace ApiContracts.Media;

// Settings that the dashboard reads from the API.
public class MediaUploadRuleDto
{
    public string MediaType { get; set; } = "";

    public List<string> AllowedExtensions { get; set; } = new();

    public long MaxBytes { get; set; }
}