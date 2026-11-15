namespace Entities;

///Data of one "Add Media" request is stored as JSON, the real is created with approve

public class MediaSubmission
{
    public MediaType MediaType { get; set; }

    ///Codes of the technologies the media belongs to. At least one.
    public required List<string> TechnologyCodes { get; set; }

    /// English, Spanish and French
    public required LocalizedText Title { get; set; }

    /// Optional, 3 languages if
    public LocalizedText? Description { get; set; }

    /// Data server gave the file. Used to find it later
    public required string StoredFileName { get; set; }

    ///Name the staff member uploaded
    public required string OriginalFileName { get; set; }

    /// file typee
    public required string ContentType { get; set; }
}