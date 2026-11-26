using Entities;

namespace Services;

/// Thrown when an item already has a pending or returned change request (UC04 ALT1).
public class OpenRequestExistsException(int requestId, ChangeStatus status)
    : Exception($"This item already has an open change request (#{requestId}, {status}).")
{
    public int RequestId { get; } = requestId;
    public ChangeStatus Status { get; } = status;
}