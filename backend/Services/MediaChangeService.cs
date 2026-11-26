using Entities;
using RepositoryContracts;

namespace Services;

public class MediaChangeService(
    IMediaRepository mediaRepository,
    ITechnologyRepository technologyRepository,
    IFileRepository fileRepository,
    IMediaFileValidator fileValidator,
    IOpenRequestGuard openRequestGuard,
    IChangeRequestSubmitter submitter) : IMediaChangeService
{
    public async Task EnsureCanChangeAsync(int mediaId)
    {
        await mediaRepository.GetSingleAsync(mediaId); // 404 when missing
        await openRequestGuard.EnsureNoOpenRequestAsync(ChangeTarget.Media, mediaId);
    }

    public async Task<ChangeRequest> SubmitUpdateAsync(
        int mediaId, UpdateMediaCommand command, CancellationToken cancellationToken)
    {
        // Blocked items are refused before anything is validated or stored.
        await EnsureCanChangeAsync(mediaId);

        ValidationCollector validation = new();

        MediaType? mediaType = null;
        if (command.Content is not null)
        {
            mediaType = fileValidator.Check(command.FileName ?? "", command.Length, validation);
        }

        LocalizedTextValidator.Required("Title", command.Title, validation);
        LocalizedTextValidator.OptionalAllOrNone("Description", command.Description, validation);
        await MediaSubmissionRules.CheckTechnologiesAsync(technologyRepository, command.TechnologyTypes, validation);

        validation.ThrowIfAny();

        string? storedFileName = null;
        if (command.Content is not null)
        {
            storedFileName = await fileRepository.SaveAsync(command.Content, command.FileName!, cancellationToken);
        }

        // File fields stay null when the current file is kept.
        MediaSubmission submission = new()
        {
            TechnologyTypes = command.TechnologyTypes.Distinct().ToList(),
            Title = command.Title!,
            Description = MediaSubmissionRules.NullIfEmpty(command.Description),
            MediaType = mediaType,
            StoredFileName = storedFileName,
            OriginalFileName = storedFileName is null ? null : command.FileName,
            ContentType = storedFileName is null
                ? null
                : string.IsNullOrWhiteSpace(command.ContentType) ? "application/octet-stream" : command.ContentType
        };

        List<string> storedFiles = [];
        if (storedFileName is not null)
        {
            storedFiles.Add(storedFileName);
        }

        return await submitter.SubmitAsync(ChangeTarget.Media, ChangeType.Update, mediaId, submission, storedFiles);
    }

    public async Task<ChangeRequest> SubmitDeleteAsync(int mediaId)
    {
        await EnsureCanChangeAsync(mediaId);
        return await submitter.SubmitAsync<MediaSubmission>(ChangeTarget.Media, ChangeType.Delete, mediaId, null, []);
    }
    
    public Task<List<ChangeRequest>> GetOpenRequestsAsync() =>
        openRequestGuard.GetOpenAsync(ChangeTarget.Media);
}