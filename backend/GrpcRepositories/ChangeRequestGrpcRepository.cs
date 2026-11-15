using Entities;
using RepositoryContracts;

namespace GrpcRepositories;

/// Stores change requests on the Java data server over gRPC.
public class ChangeRequestGrpcRepository(Generated.ChangeRequestData.ChangeRequestDataClient client)
    : IChangeRequestRepository
{
    public Task<ChangeRequest> CreateAsync(ChangeRequest request) =>
        GrpcCall.RunAsync(async () =>
        {
            Generated.ChangeRequest reply = await client.CreateAsync(ChangeRequestMapper.ToProto(request));
            return ChangeRequestMapper.ToEntity(reply);
        });
}