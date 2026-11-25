using Entities;

namespace Services;

/// One rule for every target (media, news, country...): an item with an open change request cannot get another.
public interface IOpenRequestGuard
{
    /// Throws OpenRequestExistsException when a pending or returned request exists for the item.
    Task EnsureNoOpenRequestAsync(ChangeTarget target, int targetId);
}