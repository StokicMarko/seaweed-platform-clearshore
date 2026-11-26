using Entities;

namespace GrpcRepositories;

internal static class CommonMapper //shared message types (reuse), GRPC repo use only
{
   //GRPC message to entity, recieve
    public static LocalizedText ToEntity(Generated.LocalizedText text) => new(text.En, text.Es, text.Fr);

    //Entity ro Grpc, sent (to data server)
    public static Generated.LocalizedText ToProto(LocalizedText text) => new()
    {
        En = text.En,
        Es = text.Es,
        Fr = text.Fr
    };
}
