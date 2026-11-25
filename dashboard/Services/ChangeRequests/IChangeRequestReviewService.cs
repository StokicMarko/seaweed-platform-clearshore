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
}