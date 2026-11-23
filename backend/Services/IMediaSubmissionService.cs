using Entities;

namespace Services;

/// Add Media(UC03): staff propose a new media item, a Main Admin approves it later.
public interface IMediaSubmissionService
{
    /// Checks the rules, stores the file and creates a pending change request.
    /// Throws ValidationFailedException when a rule is broken, listing every problem.
    /// Returns the created request, so the caller can show its id.
    
    Task<ChangeRequest> SubmitAsync(AddMediaCommand command, CancellationToken cancellationToken);
}