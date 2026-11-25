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
    
    
    /// Proposes changes to an existing item. No file means the current file is kept.
    Task<MediaSubmissionResult> UpdateMediaAsync(
        int id,
        MediaFormData form,
        IBrowserFile? file,
        long maximumFileBytes,
        CancellationToken cancellationToken = default);

    /// Proposes the deletion of an existing item.
    Task<MediaSubmissionResult> RequestDeleteAsync(
        int id,
        CancellationToken cancellationToken = default);
}
