namespace ApiContracts.ChangeRequests;

/// A proposed change waiting for approval. (Returned by every submition endpoint (not only for media).
/// used as one row of the review list.
public class ChangeRequestDto //shape of data that crosses the REST boundary between Tier 1 and Tier 2(dont forgett)
{
    public int Id { get; set; }
    public required string TargetType { get; set; }
    public required string Type { get; set; }
    
    // Id of the existing item. Empty for a Create.
    public int? TargetId { get; set; }
    
    public required string Status { get; set; }
    
    
    /// Id of the member who submitted it.
    public int RequestedBy { get; set; }
    
    public DateTimeOffset CreatedAt { get; set; }
}