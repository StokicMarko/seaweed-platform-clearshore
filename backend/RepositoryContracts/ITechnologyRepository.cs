using Entities;

namespace RepositoryContracts;

public interface ITechnologyRepository
{
    Task<List<Technology>> GetManyAsync();
}
