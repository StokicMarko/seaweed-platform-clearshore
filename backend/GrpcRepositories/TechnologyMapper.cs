using Entities;

namespace GrpcRepositories;

internal static class TechnologyMapper
{
    public static Technology ToEntity(Generated.Technology technology) => new()
    {
       type = technology.type;
    };
}
