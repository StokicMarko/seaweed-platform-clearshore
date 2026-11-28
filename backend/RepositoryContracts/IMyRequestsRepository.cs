using Entities;

namespace RepositoryContracts;

public interface IMyRequestsRepository
{
    /// All requests submitted by one staff member.
    Task<List<ChangeRequest>> GetByRequesterAsync(int requestedBy);

    /// Replaces the payload and status of one request.
    Task<ChangeRequest> UpdateAsync(int id, string payloadJson, ChangeStatus status);
}