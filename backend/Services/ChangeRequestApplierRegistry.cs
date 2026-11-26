using Entities;

namespace Services;

/// Collects every applier and finds the right one by target type.
/// DI hands in ALL registered appliers (IEnumerable), so a new target needs no edit here (open-closed).
public class ChangeRequestApplierRegistry : IChangeRequestApplierRegistry
{
    private readonly Dictionary<ChangeTarget, IChangeRequestApplier> appliers = new();

    public ChangeRequestApplierRegistry(IEnumerable<IChangeRequestApplier> registeredAppliers)
    {
        foreach (IChangeRequestApplier applier in registeredAppliers)
        {
            // TryAdd gives false when the target is already in the dictionary.
            if (!appliers.TryAdd(applier.TargetType, applier))
            {
                throw new InvalidOperationException($"Two appliers are registered for {applier.TargetType}.");
            }
        }
    }
                            //looks target up
    public IChangeRequestApplier GetFor(ChangeTarget targetType) =>
        appliers.TryGetValue(targetType, out IChangeRequestApplier? applier)
            ? applier
            : throw new InvalidOperationException($"No applier is registered for {targetType}.");
}