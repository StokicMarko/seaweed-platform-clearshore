using ApiContracts.ChangeRequests;

namespace Dashboard.Services.MyRequests;

public interface IMyRequestsReader
{
    Task<List<ChangeRequestDto>> GetManyAsync(
        CancellationToken cancellationToken = default);

    Task<ChangeRequestDetailDto> GetSingleAsync(
        int id,
        CancellationToken cancellationToken = default);

    /// The address of a file of one of my requests.
    string GetFileUrl(int id, string storedFileName);
}