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
            maximumFileBytes,
            cancellationToken);

        using MultipartFormDataContent multipart =
            MediaRequestBuilder.Build(form, file, stream);

        using HttpResponseMessage response = await http.PostAsync(
            "api/media",
            multipart,
            cancellationToken);

        return await ReadSubmissionAsync(response, cancellationToken);
    }

    public async Task<MediaSubmissionResult> UpdateMediaAsync(
        int id,
        MediaFormData form,
        IBrowserFile? file,
        long maximumFileBytes,
        CancellationToken cancellationToken = default)
    {
        // No file means "keep the current file": the form is sent without a File part.
        await using Stream? stream = file?.OpenReadStream(
            maximumFileBytes,
            cancellationToken);

        using MultipartFormDataContent multipart =
            MediaRequestBuilder.Build(form, file, stream);

        using HttpResponseMessage response = await http.PutAsync(
            $"api/media/{id}",
            multipart,
            cancellationToken);

        return await ReadSubmissionAsync(response, cancellationToken);
    }

    public async Task<MediaSubmissionResult> RequestDeleteAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        using HttpResponseMessage response = await http.DeleteAsync(
            $"api/media/{id}",
            cancellationToken);

        return await ReadSubmissionAsync(response, cancellationToken);
    }

    // The one place that reads the answer of every submission (add, update, delete).
    private static async Task<MediaSubmissionResult> ReadSubmissionAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        // Only confirm submission when the API returns a pending request.
        if (response.StatusCode == HttpStatusCode.Accepted)
        {
            ChangeRequestDto? request = await response.Content
                .ReadFromJsonAsync<ChangeRequestDto>(
                    cancellationToken: cancellationToken);

            if (request is not null &&
                request.Id > 0 &&
                string.Equals(
                    request.Status,
                    "Pending",
                    StringComparison.OrdinalIgnoreCase))
            {
                return new MediaSubmissionResult
                {
                    Request = request
                };
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
            Errors = await ApiErrorReader.ReadAsync(
                response,
                cancellationToken)
        };
    }
}