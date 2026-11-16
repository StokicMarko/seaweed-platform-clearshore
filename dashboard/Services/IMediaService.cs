using ApiContracts.Media;

namespace Dashboard.Services;

public interface IMediaService
{
    Task<List<MediaDto>> GetManyAsync();
    string GetFileUrl(MediaDto media);
}
