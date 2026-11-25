using ApiContracts.Media;

namespace Dashboard.Services.Media;

public interface IMediaReader
{
    Task<List<MediaDto>> GetManyAsync();

    Task<MediaDto> GetSingleAsync(
        int id,
        CancellationToken cancellationToken = default);

    // An empty string means changes are available.
    Task<string> GetBlockingRequestStatusAsync(
        int id,
        CancellationToken cancellationToken = default);

    string GetFileUrl(MediaDto media);
}