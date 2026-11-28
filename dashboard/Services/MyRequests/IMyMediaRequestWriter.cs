using Dashboard.Models;
using Dashboard.Services.Media;
using Microsoft.AspNetCore.Components.Forms;

namespace Dashboard.Services.MyRequests;

public interface IMyMediaRequestWriter
{
    /// Resubmits a returned media request. No file means the current files are kept.
    Task<MediaSubmissionResult> ResubmitAsync(
        int id,
        MediaFormData form,
        IBrowserFile? file,
        long maximumFileBytes,
        CancellationToken cancellationToken = default);
}