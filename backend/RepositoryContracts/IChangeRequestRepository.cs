using Entities;

namespace RepositoryContracts;
//List of things tier 2 can ask of the stored change req
//mirror of proto create, many, single, update, delete
/// Stores change requests through the data server. 
public interface IChangeRequestRepository {//services depend on this not on grpc (dependency injection)
    
    /// Saves a new request and returns it with its id filled in. (add media in 3 tier)
    Task<ChangeRequest> CreateAsync(ChangeRequest request);
    
    /// Returns the requests that match the filters. filter null = not applied.
    Task<List<ChangeRequest>> GetManyAsync(ChangeTarget? targetType, int? targetId, ChangeStatus? status);

    /// Returns one request. 
    Task<ChangeRequest> GetSingleAsync(int id);

    /// Changes only the status and returns the updated request.
    Task<ChangeRequest> UpdateStatusAsync(int id, ChangeStatus status);

    /// Deletes one request.
    Task DeleteAsync(int id);
}