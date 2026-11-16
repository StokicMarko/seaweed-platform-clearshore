using Entities;

namespace RepositoryContracts;

public interface IMediaRepository
{
    Task<List<Media>> GetManyAsync(string? machineCode);
    Task<Media> GetSingleAsync(int id);
    Task CopyFileToAsync(int id, Stream destination, CancellationToken cancellationToken);
}
