using System.Text.Json;
using System.Net;
using System.Net.Http.Json;
using ApiContracts.ChangeRequests;
using ApiContracts.Media;
using Dashboard.Models;
using Dashboard.Services;
using Microsoft.AspNetCore.Components.Forms;

namespace Dashboard.Services.Media;

public class HttpMediaService(HttpClient http)
    : IMediaReader, IUploadRulesService, IMediaWriter
{
    public async Task<List<MediaDto>> GetManyAsync()
    {
        return await http.GetFromJsonAsync<List<MediaDto>>(
            "api/media") ?? new();
    }

    // Retrieve the existing media item for the Edit page.
    public async Task<MediaDto> GetSingleAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        using HttpResponseMessage response = await http.GetAsync(
            $"api/media/{id}",
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errors = await ApiErrorReader.ReadAsync(
                response,
                cancellationToken);

            string message = string.Join(" ", errors);

            if (string.IsNullOrWhiteSpace(message))
            {
                message = "The media item could not be loaded.";
            }

            throw new HttpRequestException(
                message,
                null,
                response.StatusCode);
        }

        MediaDto? media = await response.Content
            .ReadFromJsonAsync<MediaDto>(
                cancellationToken: cancellationToken);

        return media ?? throw new InvalidOperationException(
            "The server returned an empty media response.");
    }

    public async Task<string> GetBlockingRequestStatusAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        using HttpResponseMessage response = await http.GetAsync(
            $"api/media/{id}/change-availability",
            cancellationToken);

        if (response.StatusCode == HttpStatusCode.Conflict)
        {
            JsonElement problem = await response.Content.ReadFromJsonAsync<JsonElement>(
                cancellationToken: cancellationToken);

            if (problem.TryGetProperty("requestStatus", out JsonElement value) &&
                value.ValueKind == JsonValueKind.String &&
                value.GetString() is "Pending" or "Returned")
            {
                return value.GetString()!;
            }

            throw new InvalidOperationException("The API did not identify the open request status.");
        }

        response.EnsureSuccessStatusCode();
        return "";
    }

    public string GetFileUrl(MediaDto media)
    {
        return new Uri(
            http.BaseAddress!,
            media.FileUrl).ToString();
    }

    public async Task<List<MediaUploadRuleDto>> GetUploadRulesAsync(
        CancellationToken cancellationToken = default)
    {
        return await http.GetFromJsonAsync<List<MediaUploadRuleDto>>(
            "api/media/upload-rules",
            cancellationToken) ?? new();
                }

    public async Task<MediaSubmissionResult> AddMediaAsync(
        MediaFormData form,
        IBrowserFile file,
        long maximumFileBytes,
        CancellationToken cancellationToken = default)
    {
        await using Stream stream = file.OpenReadStream(
            maximumFileBytes, cancellationToken);

        using MultipartFormDataContent multipart =
            MediaRequestBuilder.Build(form, file, stream);

        using HttpResponseMessage response = await http.PostAsync(
            "api/media", multipart, cancellationToken);

        return await ReadSubmissionAsync(response, cancellationToken);
    }

    public async Task<MediaSubmissionResult> UpdateMediaAsync(
        int id,
        MediaFormData form,
        IBrowserFile? file,
        long maximumFileBytes,
        CancellationToken cancellationToken = default)
    {
        await using Stream? stream = file?.OpenReadStream(
            maximumFileBytes, cancellationToken);

        using MultipartFormDataContent multipart =
            MediaRequestBuilder.Build(form, file, stream);

        using HttpResponseMessage response = await http.PutAsync(
            $"api/media/{id}", multipart, cancellationToken);

        return await ReadSubmissionAsync(response, cancellationToken);
    }

    public async Task<MediaSubmissionResult> DeleteMediaAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        using HttpResponseMessage response = await http.DeleteAsync(
            $"api/media/{id}", cancellationToken);

        return await ReadSubmissionAsync(response, cancellationToken);
    }

    private static async Task<MediaSubmissionResult> ReadSubmissionAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        if (response.StatusCode == HttpStatusCode.Conflict)
        {
            return new MediaSubmissionResult
            {
                Errors = new() { "This item already has an open change request." }
            };
        }

        if (response.StatusCode == HttpStatusCode.Accepted)
        {
            ChangeRequestDto? request =
                await response.Content.ReadFromJsonAsync<ChangeRequestDto>(
                    cancellationToken: cancellationToken);

            if (request is not null &&
                request.Id > 0 &&
                string.Equals(
                    request.Status, "Pending", StringComparison.OrdinalIgnoreCase))
            {
                return new MediaSubmissionResult { Request = request };
            }

            return new MediaSubmissionResult
            {
                Errors = new()
                {
                    "The server reply did not confirm a pending request. Check My requests before retrying."
                }
            };
        }

        return new MediaSubmissionResult
        {
            Errors = await ApiErrorReader.ReadAsync(response, cancellationToken)
        };
    }
}