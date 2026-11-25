namespace ApiContracts.Media;

/// Tells the dashboard that a media item has a request waiting (Pending) or sent back (Returned).
/// Only ids and a status, never the proposed data, an envelope :)
public class OpenMediaRequestDto
{
    public int MediaId { get; set; }
    public required string Status { get; set; }
}