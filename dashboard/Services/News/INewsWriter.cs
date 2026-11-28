using Dashboard.Models;
using Dashboard.Services.Media;
using Microsoft.AspNetCore.Components.Forms;

namespace Dashboard.Services.News;

public interface INewsWriter
{
    /// Proposes a new news item with zero or more files.
    Task<MediaSubmissionResult> AddNewsAsync(
        NewsFormData form,
        IReadOnlyList<IBrowserFile> files,
        long maximumFileBytes,
        CancellationToken cancellationToken = default);
}