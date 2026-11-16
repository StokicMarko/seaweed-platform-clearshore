using Entities;

namespace RepositoryContracts;

/// Stores change requests through the data server. 
public interface IChangeRequestRepository
{
    /// Saves a new request and returns it with its id filled in.
    Task<ChangeRequest> CreateAsync(ChangeRequest request);
}