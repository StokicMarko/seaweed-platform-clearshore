using Entities;
using RepositoryContracts;

namespace Services;

/// The Add Media use case (UC03). Checks rules, stores the file on the data server, creates a pending change request.
/// The media item itself is only created when the Main Admin approves the request. :)

public class MediaSubmissionService(
    ITechnologyRepository technologyRepository,
    IFileRepository fileRepository,
    IMediaFileValidator fileValidator,
    IChangeRequestSubmitter submitter) : IMediaSubmissionService
{
    public async Task<ChangeRequest> SubmitAsync(AddMediaCommand command, CancellationToken cancellationToken)
    {
        // If any rule is broken, nothing is saved.
        ValidationCollector validation = new();
        MediaType? mediaType = fileValidator.Check(command.FileName, command.Length, validation);
        LocalizedTextValidator.Required("Title", command.Title, validation);
        LocalizedTextValidator.OptionalAllOrNone("Description", command.Description, validation);
        await MediaSubmissionRules.CheckTechnologiesAsync(technologyRepository, command.TechnologyTypes, validation);

        validation.ThrowIfAny();

        // Save the file; the data server chooses a unique stored name.
        string storedFileName = await fileRepository.SaveAsync(command.Content, command.FileName, cancellationToken);

        // Describe the new media item. It becomes a real item only when the Main Admin approves.
        MediaSubmission submission = new()
        {
            MediaType = mediaType!.Value, // set whenever no error was reported
            TechnologyTypes = command.TechnologyTypes.Distinct().ToList(),
            Title = command.Title!, // checked by the validator above
            Description = MediaSubmissionRules.NullIfEmpty(command.Description),
            StoredFileName = storedFileName,
            OriginalFileName = command.FileName,
            ContentType = string.IsNullOrWhiteSpace(command.ContentType)
                ? "application/octet-stream"
                : command.ContentType
        };

        return await submitter.SubmitAsync(ChangeTarget.Media, ChangeType.Create, null, submission, [storedFileName]);
    }
}