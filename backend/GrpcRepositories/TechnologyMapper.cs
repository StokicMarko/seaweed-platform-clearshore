using Entities;

namespace GrpcRepositories;

internal static class TechnologyMapper
{
    public static Technology ToEntity(Generated.Technology technology) => new() { Type = technology.Type };
}
