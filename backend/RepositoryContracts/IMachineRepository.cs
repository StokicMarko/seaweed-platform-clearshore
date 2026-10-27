using Entities;

namespace RepositoryContracts;

public interface IMachineRepository
{
    Task<List<Machine>> GetManyAsync();
}
