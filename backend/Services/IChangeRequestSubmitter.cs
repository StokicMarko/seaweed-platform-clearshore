using Entities;

namespace Services;

///Last step of every use case: store a proposed change as a pending change request.
/// IChangeRequestSubmitter= staff submit, IChangeRequestReviewService = only the Main Admin.
public interface IChangeRequestSubmitter
{
    /// Saves the payload as a pending request from the current user.
    /// If saving fails, the files already stored for it are deleted and the error is rethrown.
    Task<ChangeRequest> SubmitAsync<TPayload>(
        ChangeTarget target,
        ChangeType type,
        int? targetId,
        TPayload? payload,
        IReadOnlyCollection<string> storedFileNames);
}