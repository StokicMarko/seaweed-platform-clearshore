using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using ApiContracts.ChangeRequests;
using ApiContracts.Media;
using Dashboard.Models;
using Microsoft.AspNetCore.Components.Forms;

namespace Dashboard.Services;

public class HttpMediaService(HttpClient http) : IMediaService
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
        // Use the API's limit and stream the file.
        await using Stream stream = file.OpenReadStream(
            maximumFileBytes,
            cancellationToken);

        using MultipartFormDataContent multipart = new();
        using StreamContent fileContent = new(stream);

        fileContent.Headers.ContentType =
            MediaTypeHeaderValue.TryParse(
                file.ContentType,
                out var parsedContentType)
                ? parsedContentType
                : new MediaTypeHeaderValue(
                    "application/octet-stream");

        multipart.Add(
            fileContent,
            "File",
            Path.GetFileName(file.Name));

                foreach (string type in form.TechnologyTypes)
                {
                    multipart.Add(
                        new StringContent(type),
                        "TechnologyTypes");
                }

        multipart.Add(
            new StringContent(form.TitleEn.Trim()),
            "TitleEn");

        multipart.Add(
            new StringContent(form.TitleEs.Trim()),
            "TitleEs");

        multipart.Add(
            new StringContent(form.TitleFr.Trim()),
            "TitleFr");

        multipart.Add(
            new StringContent(form.DescriptionEn.Trim()),
            "DescriptionEn");

        multipart.Add(
            new StringContent(form.DescriptionEs.Trim()),
            "DescriptionEs");

        multipart.Add(
            new StringContent(form.DescriptionFr.Trim()),
            "DescriptionFr");

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
