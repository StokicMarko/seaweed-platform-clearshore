using ApiContracts.Media;

namespace Dashboard.Services.Media;

public interface IMediaReader
{
    Task<List<MediaDto>> GetManyAsync();

    Task<MediaDto> GetSingleAsync(
        int id,
        CancellationToken cancellationToken = default);

    string GetFileUrl(MediaDto media);
}