using ApiContracts;
using ApiContracts.Machine;
using ApiContracts.Media;
using Entities;

namespace WebApi.Mappers;

public static class DtoMapper
{
    public static MediaDto ToDto(Media media) => new()
    {
        Id = media.Id,
        Machine = media.Machine is null ? null : ToDto(media.Machine),
        MediaType = media.MediaType.ToString(),
        Title = ToDto(media.Title),
        Description = media.Description is null ? null : ToDto(media.Description),
        FileName = media.OriginalFileName,
        ContentType = media.ContentType,
        FileUrl = $"/api/media/{media.Id}/file",
        PublishedAt = media.PublishedAt
    };

    public static MachineDto ToDto(Machine machine) => new()
    {
        Id = machine.Id,
        Code = machine.Code,
        Name = machine.Name
    };

    public static LocalizedTextDto ToDto(LocalizedText text) => new(text.En, text.Es, text.Fr);
}
