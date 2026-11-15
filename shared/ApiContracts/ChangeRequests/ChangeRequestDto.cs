namespace ApiContracts.ChangeRequests;

/// A proposed change waiting for approval. (Returned by every sumbition endpoint (not only for media).
public class ChangeRequestDto
{
    public int Id { get; set; }
    public required string Target { get; set; }
    public required string Action { get; set; }
    public required string Status { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}