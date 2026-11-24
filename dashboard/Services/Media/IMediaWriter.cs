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
}
