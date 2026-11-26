using System.Text.Json;
using Entities;
using RepositoryContracts;

namespace Services;

/// Applies approved MEDIA change requests (create, update, delete). The ONLYY class that knows the media payload. ( I swear this responsability separation is killing me.
/// SOLID: S = media only. D = gets its repositories through the constructor (DI), so tests can use fakes.
public class MediaChangeRequestApplier(
    IMediaRepository mediaRepository,
    IFileRepository fileRepository) : IChangeRequestApplier
{
    public ChangeTarget TargetType => ChangeTarget.Media;

    //Cancelletaion token = stop button for request
    //Repos take no cancellation token (yet at least), stays so the interface is tha same foe every applier .
    public async Task ApplyAsync(ChangeRequest request, CancellationToken cancellationToken)
    {
        switch (request.Type)
        {
            case ChangeType.Create:
                await ApplyCreateAsync(request);
                break;
            case ChangeType.Update:
                await ApplyUpdateAsync(request);
                break;
            case ChangeType.Delete:
                await ApplyDeleteAsync(request);
                break;
            default:
                throw new InvalidOperationException($"Unknown change type {request.Type}.");
        }
    }

    // Decline: remove the file that was uploaded for this request.
    public async Task CleanupAsync(ChangeRequest request, CancellationToken cancellationToken)
    {
        // A delete request has no payload and no new file.
        if (request.Type == ChangeType.Delete)
        {
            return;
        }

        MediaSubmission submission;
        try
        {
            submission = ReadPayload(request);
        }
        catch (ValidationFailedException)
        {
            // A broken payload must not stop the admin from declining. Nothing is known about a file, decline continues. 
            return;
        }

        // Not set = the update kept the current file, which must NOT be deleted.
        if (submission.StoredFileName is not null)
        {
            await TryDeleteFileAsync(submission.StoredFileName);
        }
    }

    // Admin preview: the file uploaded for this request (none for a delete, or an update that keeps the file).
    public IReadOnlyList<RequestFile> GetFiles(ChangeRequest request)
    {
        // A delete request carries no new file.
        if (request.Type == ChangeType.Delete)
        {
            return [];
        }

        MediaSubmission submission;
        try
        {
            submission = ReadPayload(request);
        }
        catch (ValidationFailedException)
        {
            // A broken payload has no known file. The preview shows nothing instead of failing.
            return [];
        }

        // Not set = the update kept the current file, so this request carries no new file.
        if (string.IsNullOrWhiteSpace(submission.StoredFileName)
            || string.IsNullOrWhiteSpace(submission.OriginalFileName)
            || string.IsNullOrWhiteSpace(submission.ContentType))
        {
            return [];
        }

        return [new RequestFile(submission.StoredFileName, submission.OriginalFileName, submission.ContentType)];
    }

    private async Task ApplyCreateAsync(ChangeRequest request)
    {
        MediaSubmission submission = ReadPayload(request);

        // A new media item needs its file: all four file fields must be there.
        ValidationCollector validation = new();
        CheckAllFileFieldsPresent(submission, validation);
        validation.ThrowIfAny();

        // Safe to repeat: the data server returns the existing item for the same stored file name.
        await mediaRepository.CreateAsync(submission);
    }

    private async Task ApplyUpdateAsync(ChangeRequest request)
    {
        int id = RequireTargetId(request);
        MediaSubmission submission = ReadPayload(request);

        ValidationCollector validation = new();
        CheckFileFieldsAllOrNone(submission, validation);
        validation.ThrowIfAny();

        // Read first: we need the old stored file name to delete it later. NotFound = 404.
        Media current = await mediaRepository.GetSingleAsync(id);

        await mediaRepository.UpdateAsync(id, submission);

        // Only a new file name means the old file is now unused. On a repeat the names already match.
        if (submission.StoredFileName is not null && submission.StoredFileName != current.StoredFileName)
        {
            await TryDeleteFileAsync(current.StoredFileName);
        }
    }

    private async Task ApplyDeleteAsync(ChangeRequest request)
    {
        int id = RequireTargetId(request);

        Media current;
        try
        {
            current = await mediaRepository.GetSingleAsync(id);
            await mediaRepository.DeleteAsync(id);
        }
        catch (NotFoundException)
        {
            // Already deleted by an earlier approval: the goal is reached, so repeating is safe.
            return;
        }

        // Row first, file second: the worst case is an unused file, never a row without its file.
        await TryDeleteFileAsync(current.StoredFileName);
    }

    // Reads the JSON with the same settings the submitter wrote it with.
    private static MediaSubmission ReadPayload(ChangeRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.PayloadJson))
        {
            throw new ValidationFailedException(["The change request has no payload."]);
        }

        try
        {
            return JsonSerializer.Deserialize<MediaSubmission>(request.PayloadJson, ChangeRequestJson.Options)
                   ?? throw new ValidationFailedException(["The change request payload is empty."]);
        }
        catch (JsonException)
        {
            throw new ValidationFailedException(["The change request payload is not valid."]);
        }
    }

    private static int RequireTargetId(ChangeRequest request) =>
        request.TargetId ?? throw new ValidationFailedException(["The change request has no target id."]);

    private static void CheckAllFileFieldsPresent(MediaSubmission submission, ValidationCollector validation)
    {
        if (submission.MediaType is null)
        {
            validation.Add("The media type is missing.");
        }

        if (IsMissing(submission.OriginalFileName))
        {
            validation.Add("The original file name is missing.");
        }

        if (IsMissing(submission.StoredFileName))
        {
            validation.Add("The stored file name is missing.");
        }

        if (IsMissing(submission.ContentType))
        {
            validation.Add("The content type is missing.");
        }
    }

    // An update either brings a whole new file (all four) or keeps the current one (none).
    private static void CheckFileFieldsAllOrNone(MediaSubmission submission, ValidationCollector validation)
    {
        int filled = new[]
        {
            submission.MediaType is not null,
            !IsMissing(submission.OriginalFileName),
            !IsMissing(submission.StoredFileName),
            !IsMissing(submission.ContentType)
        }.Count(isFilled => isFilled);

        if (filled != 0 && filled != 4)
        {
            validation.Add("The file information must be complete, or left empty to keep the current file.");
        }
    }

    // The change is already done, so a failed file cleanup must not fail the approval.
    // A file that is already gone is the goal, so that counts as done too.
    private async Task TryDeleteFileAsync(string storedFileName)
    {
        try
        {
            await fileRepository.DeleteAsync(storedFileName);
        }
        catch (Exception e) when (e is DataAccessException or NotFoundException)
        {
        }
    }

    private static bool IsMissing(string? value) => string.IsNullOrWhiteSpace(value);
}