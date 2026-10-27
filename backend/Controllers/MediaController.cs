using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace Backend.Controllers;

[ApiController]
[Route("api/media")]
public class MediaController(NpgsqlDataSource dataSource) : ControllerBase
{
    private const string Query = """
        SELECT id, machine, title_en, title_fr, title_es,
               description_en, description_fr, description_es,
               file_name, url, content_type, created_at
        FROM sep3.media
        ORDER BY id
        """;

    [HttpGet]
    public async Task<List<MediaItem>> GetAll(CancellationToken cancellationToken)
    {
        await using var command = dataSource.CreateCommand(Query);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        var items = new List<MediaItem>();
        while (await reader.ReadAsync(cancellationToken))
        {
            items.Add(new MediaItem(
                reader.GetInt32(0),
                GetNullableString(reader, 1),
                reader.GetString(2),
                reader.GetString(3),
                reader.GetString(4),
                GetNullableString(reader, 5),
                GetNullableString(reader, 6),
                GetNullableString(reader, 7),
                reader.GetString(8),
                reader.GetString(9),
                reader.GetString(10),
                reader.GetFieldValue<DateTimeOffset>(11)));
        }

        return items;
    }

    private static string? GetNullableString(NpgsqlDataReader reader, int ordinal) =>
        reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);
}
