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
        LocalizedTextValidator.Required("Description", command.Description, validation);
        await CheckTechnologiesAsync(command.TechnologyCodes, validation);

        validation.ThrowIfAny();

        // Save the file; the data server chooses a unique stored name.
        string storedFileName = await fileRepository.SaveAsync(command.Content, command.FileName, cancellationToken);

        // Describe the new media item. It becomes a real item only when the Main Admin approves.
        MediaSubmission submission = new()
        {
            MediaType = mediaType!.Value, // set whenever no error was reported
            TechnologyCodes = command.TechnologyCodes.Distinct().ToList(),
            Title = command.Title!, // checked by the validator above
            Description = NullIfEmpty(command.Description),
            StoredFileName = storedFileName,
            OriginalFileName = command.FileName,
            ContentType = string.IsNullOrWhiteSpace(command.ContentType)
                ? "application/octet-stream"
                : command.ContentType
        };

        return await submitter.SubmitAsync(ChangeTarget.Media, ChangeAction.Create, submission, [storedFileName]);
    }

    // at least one technology, and every code must exist.
    // The docs call them technologies, but the code and database call them machines.(check meeting)
    private async Task CheckTechnologiesAsync(List<string> codes, ValidationCollector validation)
    {
        if (codes.Count == 0)
        {
            validation.Add("Choose at least one technology.");
            return;
        }

        List<Technology> technologies = await technologyRepository.GetManyAsync();
        HashSet<string> known = technologies.Select(technology => technology.Type).ToHashSet();

        foreach (string code in codes.Where(code => !known.Contains(code)).Distinct())
        {
            validation.Add($"Unknown technology '{code}'.");
        }
    }

    // Three empty texts mean "no description".
    private static LocalizedText? NullIfEmpty(LocalizedText? description) =>
        description is null ||
        (string.IsNullOrWhiteSpace(description.En) &&
         string.IsNullOrWhiteSpace(description.Es) &&
         string.IsNullOrWhiteSpace(description.Fr))
            ? null
            : description;
}