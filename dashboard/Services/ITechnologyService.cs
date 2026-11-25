using ApiContracts.Technology;

namespace Dashboard.Services;

public interface ITechnologyService
{
    Task<List<TechnologyDto>> GetManyAsync(
        CancellationToken cancellationToken = default);
}
