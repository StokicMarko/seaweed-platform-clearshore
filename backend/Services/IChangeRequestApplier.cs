using Entities;

namespace Services;

//approve request to real data. for one target type 
//each a own applier
//SOLID | knows how to apply a target

public interface IChangeRequestApplier
{
    /// target applier handles. (registry use to find the right applier.)
    ChangeTarget TargetType { get; }

    /// Request: create, update or delete the real item.
    /// Reads its own payload from request.PayloadJson -->  only this applier knows the shape.
    /// Safe to repeat (delete fail)
    Task ApplyAsync(ChangeRequest request, CancellationToken cancellationToken);

    /// Deletes the files stored for this request. Decline
    Task CleanupAsync(ChangeRequest request, CancellationToken cancellationToken);
}