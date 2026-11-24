namespace Entities;

public class Media
{
    public int Id { get; set; }
   public List<Technology> Technologies { get; set; } = [];
    public MediaType MediaType { get; set; }
    public required LocalizedText Title { get; set; }
    public LocalizedText? Description { get; set; }
    public required string OriginalFileName { get; set; }
    
    //given name from the data server, media proto carry
    public required string StoredFileName { get; set; }
    public required string ContentType { get; set; }
    public DateTimeOffset PublishedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}
