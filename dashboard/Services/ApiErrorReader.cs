using System.Net;
using System.Text.Json;

namespace Dashboard.Services;

public static class ApiErrorReader
{
    public static async Task<List<string>> ReadAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken = default)
    {
        List<string> errors = new();

        // 400 carries the field errors; 409 (open request), 403 and 404 carry the reason in the title.
        if (response.StatusCode is HttpStatusCode.BadRequest
            or HttpStatusCode.Conflict
            or HttpStatusCode.Forbidden
            or HttpStatusCode.NotFound)
        {
            try
            {
                string responseText =
                    await response.Content.ReadAsStringAsync(
                        cancellationToken);

                using JsonDocument document =
                    JsonDocument.Parse(responseText);

                if (document.RootElement.TryGetProperty(
                    "errors",
                    out JsonElement details))
                {
                    if (details.ValueKind == JsonValueKind.Array)
                    {
                        ReadMessages(details, errors);
                    }
                    else if (details.ValueKind == JsonValueKind.Object)
                    {
                        foreach (JsonProperty property
                                 in details.EnumerateObject())
                        {
                            ReadMessages(property.Value, errors);
                        }
                    }
                }

                if (errors.Count == 0 &&
                    document.RootElement.TryGetProperty(
                        "title",
                        out JsonElement title) &&
                    title.ValueKind == JsonValueKind.String)
                {
                    errors.Add(
                        title.GetString() ??
                        "The submission was rejected.");
                }
            }
            catch (JsonException)
            {
            }
        }

        if (errors.Count == 0)
        {
            if (response.StatusCode ==
                HttpStatusCode.RequestEntityTooLarge)
            {
                errors.Add(
                    "The server rejected the upload because it is too large. Check the displayed file limit.");
            }
            else
            {
                errors.Add(
                    $"The submission could not be confirmed (HTTP {(int)response.StatusCode}). Check My requests before retrying.");
            }
        }

        return errors;
    }

    private static void ReadMessages(
        JsonElement value,
        List<string> errors)
    {
        if (value.ValueKind == JsonValueKind.String &&
            !string.IsNullOrWhiteSpace(value.GetString()))
        {
            errors.Add(value.GetString()!);
        }
        else if (value.ValueKind == JsonValueKind.Array)
        {
            foreach (JsonElement item in value.EnumerateArray())
            {
                ReadMessages(item, errors);
            }
        }
    }
}
