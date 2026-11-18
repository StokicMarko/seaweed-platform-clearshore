using ApiContracts.Media;
using Dashboard.Models;
using Microsoft.AspNetCore.Components.Forms;

namespace Dashboard.Services;

public interface IMediaService
{
    Task<List<MediaDto>> GetManyAsync();

    string GetFileUrl(MediaDto media);

    Task<List<MediaUploadRuleDto>> GetUploadRulesAsync(
        CancellationToken cancellationToken = default);

    Task<MediaSubmissionResult> AddMediaAsync(
        MediaFormData form,
        IBrowserFile file,
        long maximumFileBytes,
        CancellationToken cancellationToken = default);
}