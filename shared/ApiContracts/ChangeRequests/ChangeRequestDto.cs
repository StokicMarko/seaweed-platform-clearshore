namespace ApiContracts.ChangeRequests;

/// A proposed change waiting for approval. (Returned by every sumbition endpoint (not only for media).
public class ChangeRequestDto
{
    public int Id { get; set; }
    public required string TargetType { get; set; }
    public required string Type { get; set; }
    public required string Status { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}