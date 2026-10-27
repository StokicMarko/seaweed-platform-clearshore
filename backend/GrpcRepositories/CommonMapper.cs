using Entities;

namespace GrpcRepositories;

internal static class CommonMapper
{
    public static LocalizedText ToEntity(Generated.LocalizedText text) => new(text.En, text.Es, text.Fr);
}
