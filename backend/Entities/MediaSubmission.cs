namespace Entities;

///Holds full Data of a media change request (add edit/ CREATE UPDATE), request is stored as JSON = payload of the change request, the real is created with approve)

public class MediaSubmission 
{
   
    ///Codes of the technologies the media belongs to. At least one.
    public required List<string> TechnologyTypes { get; set; }

    /// English, Spanish and French
    public required LocalizedText Title { get; set; }

    /// Optional, 3 languages if
    public LocalizedText? Description { get; set; }

   //For Update they are all null when the staff member keeps the current file. ->

        
    public MediaType? MediaType { get; set; } //MEDIA optional for edit but checked for add applier
    
    
    /// Data server gave the file. Used to find it later
    public string? StoredFileName { get; set; }

    ///Name the staff member uploaded
    public string? OriginalFileName { get; set; }

    /// file typee
    public string? ContentType { get; set; }
    
    
    
}