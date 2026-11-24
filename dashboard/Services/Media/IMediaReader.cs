using ApiContracts.Media;

namespace Dashboard.Services.Media;

public interface IMediaReader
{
    Task<List<MediaDto>> GetManyAsync();

    string GetFileUrl(MediaDto media);
}
