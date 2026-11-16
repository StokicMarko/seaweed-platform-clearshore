using Entities;
using RepositoryContracts;

namespace GrpcRepositories;

public class MachineGrpcRepository(Generated.MachineData.MachineDataClient client) : IMachineRepository
{
    public Task<List<Machine>> GetManyAsync() =>
        GrpcCall.RunAsync(async () =>
        {
            Generated.MachineList reply = await client.GetManyAsync(new Generated.GetManyMachinesRequest());
            return reply.Items.Select(MachineMapper.ToEntity).ToList();
        });
}
