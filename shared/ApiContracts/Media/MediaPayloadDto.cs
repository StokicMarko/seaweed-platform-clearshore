namespace ApiContracts.ChangeRequests;

public class MediaPayloadDto {//tier 1 view of media payload, when target type = media 
//                            The data inside the payload of a media change request (Create or Update).
//Mediasubmission entity= tier 2 --> twin but dasboard
// field names same as stored in json so no mapper


    public List<string> TechnologyTypes { get; set; } = [];

    public required LocalizedTextDto Title { get; set; }

    public LocalizedTextDto? Description { get; set; }

    // set for Create. Update = all null when the staff member keeps the current file.

    
    public string? MediaType { get; set; }

    /// Name the data server gave the file.
    public string? StoredFileName { get; set; }
    
    /// Name the staff member uploaded.
    public string? OriginalFileName { get; set; }

    /// File type, for example image/png.
    public string? ContentType { get; set; }
}