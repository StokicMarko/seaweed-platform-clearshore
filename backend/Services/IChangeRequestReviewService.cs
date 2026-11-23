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
}