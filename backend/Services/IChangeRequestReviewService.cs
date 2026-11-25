using Entities;

namespace Services;

///  Main Admin does with change requests
///  IChangeRequestSubmitter= staff submit, IChangeRequestReviewService = only the Main Admin.
/// Every method throws NotAllowedException (service) when not admin (id not existanc)
///     some    ValidationFailedException when the request is not pending.

public interface IChangeRequestReviewService
{
    /// Lists the requests pending
    Task<List<ChangeRequest>> GetPendingAsync();

    /// Returns one request  = preview
   
    Task<ChangeRequest> GetSingleAsync(int id);

    /// Approves a pending request
    ///     applies change,  deletes request.
    ///  change first, if fail the request stays and approving again is safe. (ValidationFailedException when the request is not pending.)
    Task ApproveAsync(int id);

    /// Declines a pending request deletes the request and files
    Task DeclineAsync(int id);

    /// Returns a pending request t
    Task<ChangeRequest> ReturnAsync(int id);
    
    
    /// Finds a file of a pending request for the preview (its name and content type).
    Task<RequestFile> GetFileAsync(int id, string storedFileName);

    /// Streams that file to the destination (the HTTP response). Same checks as GetFileAsync.
    Task CopyFileToAsync(int id, string storedFileName, Stream destination, CancellationToken cancellationToken);
}