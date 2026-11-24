using System.Text.Json;
using ApiContracts;
using ApiContracts.Technology;
using ApiContracts.Media;
using Entities;
using ApiContracts.ChangeRequests;

namespace WebApi.Mappers;

public static class DtoMapper //translator for Tier 2 to Tier 1
{
    public static MediaDto ToDto(Media media) => new()
    {
        Id = media.Id,
        Technologies = media.Technologies.Select(ToDto).ToList(),
        MediaType = media.MediaType.ToString(),
        Title = ToDto(media.Title),
        Description = media.Description is null ? null : ToDto(media.Description),
        FileName = media.OriginalFileName,
        ContentType = media.ContentType,
        FileUrl = $"/api/media/{media.Id}/file",
        PublishedAt = media.PublishedAt
    };

    public static TechnologyDto ToDto(Technology technology) => new()
    {
        Type = technology.Type
    };

    public static LocalizedTextDto ToDto(LocalizedText text) => new(text.En, text.Es, text.Fr);
    
    /// Turns a change request into the DTO the dashboard receives. The enums are sent as text.
    public static ChangeRequestDto ToDto(ChangeRequest request) => new()
    {
        Id = request.Id,
        TargetType = request.TargetType.ToString(),
        Type = request.Type.ToString(),
        TargetId = request.TargetId,
        Status = request.Status.ToString(),
        RequestedBy = request.RequestedBy,
        CreatedAt = request.CreatedAt
    };
    
    /// Same as above plus the proposed data - for Admin preview.
    /// The payload is stored as JSON text, so it is passed on as JSON (works for any target type).
    public static ChangeRequestDetailDto ToDetailDto(ChangeRequest request) => new()
    {
        Id = request.Id,
        TargetType = request.TargetType.ToString(),
        Type = request.Type.ToString(),
        TargetId = request.TargetId,
        Status = request.Status.ToString(),
        RequestedBy = request.RequestedBy,
        CreatedAt = request.CreatedAt,
        Payload = string.IsNullOrWhiteSpace(request.PayloadJson)
            ? null                                                       // a Delete may have no payload
            : JsonSerializer.Deserialize<JsonElement>(request.PayloadJson)   // text -> JSON element
    };
}