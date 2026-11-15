namespace Entities;
// Where a change request is in the approval flow
/// Approved requests are published and declined requests are erased, so only two states are stored.
public enum ChangeStatus
{
    Pending,
    Returned
}