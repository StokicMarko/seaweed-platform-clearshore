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

    public Task<List<ChangeRequest>> GetManyAsync(ChangeTarget? targetType, int? targetId, ChangeStatus? status) =>
        GrpcCall.RunAsync(async () =>
        {
            Generated.ChangeRequestList reply =
                await client.GetManyAsync(ChangeRequestMapper.ToGetManyRequest(targetType, targetId, status));
            return reply.Items.Select(item => ChangeRequestMapper.ToEntity(item)).ToList();
        });

    public Task<ChangeRequest> GetSingleAsync(int id) =>
        GrpcCall.RunAsync(async () =>
        {
            Generated.ChangeRequest reply = await client.GetSingleAsync(new Generated.ChangeRequestId { Id = id });
            return ChangeRequestMapper.ToEntity(reply);
        });

    public Task<ChangeRequest> UpdateStatusAsync(int id, ChangeStatus status) =>
        GrpcCall.RunAsync(async () =>
        {
            Generated.ChangeRequest reply =
                await client.UpdateStatusAsync(ChangeRequestMapper.ToUpdateStatusRequest(id, status));
            return ChangeRequestMapper.ToEntity(reply);
        });

    public Task DeleteAsync(int id) =>
        GrpcCall.RunAsync(async () =>
        {
            await client.DeleteAsync(new Generated.ChangeRequestId { Id = id });
        });
}