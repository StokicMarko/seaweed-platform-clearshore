using System.Net;
using System.Net.Http.Json;
using ApiContracts.ChangeRequests;
using Dashboard.Services;

namespace Dashboard.Services.ChangeRequests;

public class HttpChangeRequestReviewService(HttpClient http)
    : IChangeRequestReviewService
{
    public async Task<List<ChangeRequestDto>> GetPendingAsync(
        CancellationToken cancellationToken = default)
    {
        using HttpResponseMessage response = await http.GetAsync(
            "api/change-requests/pending",
            cancellationToken);

        await EnsureSuccessAsync(response, cancellationToken);

        return await ReadBodyAsync<List<ChangeRequestDto>>(
            response,
            cancellationToken);
    }

    public async Task<ChangeRequestDetailDto> GetSingleAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        using HttpResponseMessage response = await http.GetAsync(
            $"api/change-requests/{id}",
            cancellationToken);

        await EnsureSuccessAsync(response, cancellationToken);

        return await ReadBodyAsync<ChangeRequestDetailDto>(
            response,
            cancellationToken);
    }

    public Task ApproveAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return PostWithoutResponseBodyAsync(
            $"api/change-requests/{id}/approve",
            cancellationToken);
    }

    public Task DeclineAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return PostWithoutResponseBodyAsync(
            $"api/change-requests/{id}/decline",
            cancellationToken);
    }

    public async Task<ChangeRequestDto> ReturnAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        // The contract has no comment or note body.
        using HttpResponseMessage response = await http.PostAsync(
            $"api/change-requests/{id}/return",
            content: null,
            cancellationToken: cancellationToken);

        await EnsureSuccessAsync(response, cancellationToken);

        return await ReadBodyAsync<ChangeRequestDto>(
            response,
            cancellationToken);
    }

    private async Task PostWithoutResponseBodyAsync(
        string route,
        CancellationToken cancellationToken)
    {
        using HttpResponseMessage response = await http.PostAsync(
            route,
            content: null,
            cancellationToken: cancellationToken);

        await EnsureSuccessAsync(response, cancellationToken);

        // Approve and Decline return 204, not a request DTO.
        if (response.StatusCode != HttpStatusCode.NoContent)
        {
            throw new InvalidOperationException(
                "The server returned an unexpected response. " +
                "Reload the request before trying the action again.");
        }
    }

    private static async Task<T> ReadBodyAsync<T>(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
        where T : class
    {
        T? result = await response.Content.ReadFromJsonAsync<T>(
            cancellationToken: cancellationToken);

        return result ?? throw new InvalidOperationException(
            "The server returned an empty response.");
    }

    private static async Task EnsureSuccessAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        string message;

        switch (response.StatusCode)
        {
            case HttpStatusCode.Forbidden:
                message =
                    "Only the Main Admin may review change requests.";
                break;

            case HttpStatusCode.NotFound:
                message =
                    "The request or requested endpoint could not be found. " +
                    "Reload the pending requests list.";
                break;

            case HttpStatusCode.BadRequest:
                // Reuse the foundation's validation-error reader.
                var errors = await ApiErrorReader.ReadAsync(
                    response,
                    cancellationToken);

                message = string.Join(" ", errors);

                if (string.IsNullOrWhiteSpace(message))
                {
                    message =
                        "The request is no longer pending. " +
                        "Reload its details.";
                }

                break;

            case HttpStatusCode.BadGateway:
                message =
                    "The Data Server could not complete the operation. " +
                    "Reload the request before trying the action again.";
                break;

            default:
                message =
                    $"The operation could not be confirmed " +
                    $"(HTTP {(int)response.StatusCode}). " +
                    "Reload before trying again.";
                break;
        }

        throw new HttpRequestException(
            message,
            null,
            response.StatusCode);
    }
}