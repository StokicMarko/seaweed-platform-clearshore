using System.Net.Http.Json;
using ApiContracts.Technology;

namespace Dashboard.Services;

public class HttpTechnologyService(HttpClient http) : ITechnologyService
{
    public async Task<List<TechnologyDto>> GetManyAsync(
        CancellationToken cancellationToken = default)
    {
        return await http.GetFromJsonAsync<List<TechnologyDto>>(
            "api/technologies",
            cancellationToken) ?? new();
    }
}