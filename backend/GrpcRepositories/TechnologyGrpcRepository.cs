using Entities;
using RepositoryContracts;

namespace GrpcRepositories;

public class TechnologyGrpcRepository(Generated.TechnologyData.TechnologyDataClient client) : IMachineRepository
{
    public Task<List<Technology>> GetManyAsync() =>
        GrpcCall.RunAsync(async () =>
        {
            Generated.TechnologyList reply = await client.GetManyAsync(new Generated.GetManyTechnologiesRequest());
            return reply.Items.Select(TechnologyMapper.ToEntity).ToList();
        });
}
