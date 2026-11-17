using System.Net.Http.Json;
using ApiContracts.Technology;

namespace Dashboard.Services;

// Gets the technology choices from the API.
public class HttpTechnologyService
{
    private readonly HttpClient http;

    public HttpTechnologyService(HttpClient http)
    {
        this.http = http;
    }

    public async Task<List<TechnologyDto>> GetManyAsync()
    {
        var technologies =
            await http.GetFromJsonAsync<List<TechnologyDto>>(
                "api/technologies");

        return technologies ?? new List<TechnologyDto>();
    }
}