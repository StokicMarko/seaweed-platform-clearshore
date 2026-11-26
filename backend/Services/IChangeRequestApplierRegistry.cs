using Entities;

namespace Services;

/// Find applier for a target type. Review service uses this (not concrete applier) SOLID.

public interface IChangeRequestApplierRegistry
{
    /// Returns the applier registered for the target.
    /// InvalidOperationException if there is none (a class was not registered, not the user's mistake).
    IChangeRequestApplier GetFor(ChangeTarget targetType);
}