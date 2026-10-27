using ApiContracts.Media;

namespace Dashboard.Services;

public class HttpMediaService(HttpClient http) : IMediaService
{
    public async Task<List<MediaDto>> GetManyAsync() =>
        await http.GetFromJsonAsync<List<MediaDto>>("api/media") ?? [];

    public string GetFileUrl(MediaDto media) => new Uri(http.BaseAddress!, media.FileUrl).ToString();
}
