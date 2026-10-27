using Dashboard.Models;

namespace Dashboard.Services;

public class MediaClient(HttpClient http)
{
    public async Task<List<MediaItem>> GetAllAsync() =>
        await http.GetFromJsonAsync<List<MediaItem>>("api/media") ?? [];
}
