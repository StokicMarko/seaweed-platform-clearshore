using ApiContracts.Machine;

namespace ApiContracts.Media;

public class MediaDto
{
    public int Id { get; set; }
    public MachineDto? Machine { get; set; }
    public required string MediaType { get; set; }
    public required LocalizedTextDto Title { get; set; }
    public LocalizedTextDto? Description { get; set; }
    public required string FileName { get; set; }
    public required string ContentType { get; set; }
    public required string FileUrl { get; set; }
    public DateTimeOffset PublishedAt { get; set; }
}
