using Entities;

namespace GrpcRepositories;

internal static class MediaMapper
{
    public static Media ToEntity(Generated.Media media) => new()
    {
        Id = media.Id,
        Machine = media.Machine is null ? null : MachineMapper.ToEntity(media.Machine),
        MediaType = ToEntity(media.MediaType),
        Title = CommonMapper.ToEntity(media.Title),
        Description = media.Description is null ? null : CommonMapper.ToEntity(media.Description),
        OriginalFileName = media.OriginalFileName,
        ContentType = media.ContentType,
        PublishedAt = media.PublishedAt.ToDateTimeOffset(),
        UpdatedAt = media.UpdatedAt.ToDateTimeOffset()
    };

    private static MediaType ToEntity(Generated.MediaType mediaType) => mediaType switch
    {
        Generated.MediaType.Image => MediaType.Image,
        Generated.MediaType.Video => MediaType.Video,
        Generated.MediaType.Model3D => MediaType.Model3D,
        _ => throw new ArgumentOutOfRangeException(nameof(mediaType), mediaType, "Unknown media type")
    };
}
