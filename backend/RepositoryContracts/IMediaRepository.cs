using Entities;

namespace RepositoryContracts;

public interface IMediaRepository
{
    Task<List<Media>> GetManyAsync(string? technologyType);
    Task<Media> GetSingleAsync(int id);
    Task CopyFileToAsync(int id, Stream destination, CancellationToken cancellationToken);
}
