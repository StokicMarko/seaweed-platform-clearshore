using System.Net;
using System.Net.Http.Json;
using ApiContracts.ChangeRequests;
using ApiContracts.Media;
using Dashboard.Services;
using Dashboard.Models;
using Microsoft.AspNetCore.Components.Forms;

namespace Dashboard.Services.Media;

public class HttpMediaService(HttpClient http) : IMediaReader, IUploadRulesService, IMediaWriter
{
    public async Task<List<MediaDto>> GetManyAsync()
    {
        return await http.GetFromJsonAsync<List<MediaDto>>(
            "api/media") ?? new();
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

        // Show confirmation only for a real pending request.
        if (response.StatusCode == HttpStatusCode.Accepted)
        {
            ChangeRequestDto? request =
                await response.Content
                    .ReadFromJsonAsync<ChangeRequestDto>(
                        cancellationToken);

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
                    "We could not confirm that your submission was saved. Please try again in a moment."
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
