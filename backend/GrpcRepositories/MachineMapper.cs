using Entities;

namespace GrpcRepositories;

internal static class MachineMapper
{
    public static Machine ToEntity(Generated.Machine machine) => new()
    {
        Id = machine.Id,
        Code = machine.Code,
        Name = machine.Name
    };
}
