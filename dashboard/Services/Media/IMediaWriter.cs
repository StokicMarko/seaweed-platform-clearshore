using Dashboard.Models;
using Microsoft.AspNetCore.Components.Forms;

namespace Dashboard.Services.Media;

public interface IMediaWriter
{
    Task<MediaSubmissionResult> AddMediaAsync(
        MediaFormData form,
        IBrowserFile file,
        long maximumFileBytes,
        CancellationToken cancellationToken = default);

    Task<MediaSubmissionResult> UpdateMediaAsync(
        int id,
        MediaFormData form,
        IBrowserFile? file,
        long maximumFileBytes,
        CancellationToken cancellationToken = default);

    Task<MediaSubmissionResult> DeleteMediaAsync(
        int id,
        CancellationToken cancellationToken = default);
}