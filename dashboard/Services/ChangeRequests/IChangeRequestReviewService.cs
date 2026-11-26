using ApiContracts.ChangeRequests;

namespace Dashboard.Services.ChangeRequests;

public interface IChangeRequestReviewService
{
    Task<List<ChangeRequestDto>> GetPendingAsync(
        CancellationToken cancellationToken = default);

    Task<ChangeRequestDetailDto> GetSingleAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task ApproveAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task DeclineAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<ChangeRequestDto> ReturnAsync(
        int id,
        CancellationToken cancellationToken = default);
    
    /// The address of an uploaded file of a request, for the preview in the browser.
    string GetFileUrl(int id, string storedFileName);
}
