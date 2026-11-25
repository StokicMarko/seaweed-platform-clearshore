using Entities;

namespace Services;

/// Manage Media (UC04): staff propose to edit or delete an existing item; a Main Admin approves later.
/// Nothing here changes the media item itself.
public interface IMediaChangeService
{
    /// Throws NotFoundException when the item does not exist, OpenRequestExistsException when it has an open request (ALT1).
    Task EnsureCanChangeAsync(int mediaId);

    /// Checks the rules, stores a new file if one was sent, and creates a pending Update request.
    /// Throws ValidationFailedException listing every problem (ALT2, ALT3).
    Task<ChangeRequest> SubmitUpdateAsync(int mediaId, UpdateMediaCommand command, CancellationToken cancellationToken);

    /// Creates a pending Delete request.
    Task<ChangeRequest> SubmitDeleteAsync(int mediaId);
}