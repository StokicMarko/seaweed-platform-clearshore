using ApiContracts.Media;

namespace Dashboard.Services.Media;

public interface IUploadRulesService
{
    Task<List<MediaUploadRuleDto>> GetUploadRulesAsync(
        CancellationToken cancellationToken = default);
}
