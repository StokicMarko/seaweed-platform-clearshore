using System.Text.Json;
using System.Text.Json.Serialization;
using Entities;
using RepositoryContracts;

namespace Services;

/// The Add Media use case (UC03). Checks rules, stores the file on the data server, creates a pending change request.
/// The media item itself is only created when the Main Admin approves the request. :)

public class MediaSubmissionService(
    IMachineRepository machineRepository,
    IFileRepository fileRepository,
    IChangeRequestRepository changeRequestRepository,
    ICurrentUser currentUser,
    IMediaFileValidator fileValidator) : IMediaSubmissionService

{
    // The payload is stored as JSON text. Enums are written as names (for example "Video").
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    public async Task<ChangeRequest> SubmitAsync(AddMediaCommand command, CancellationToken cancellationToken)
    {
        // Check, rule is broken = no save
        ValidationCollector validation = new();
        MediaType? mediaType = fileValidator.Check(command.FileName, command.Length, validation);
        LocalizedTextValidator.Required("Title", command.Title, validation);
        LocalizedTextValidator.OptionalAllOrNone("Description", command.Description, validation);
        await CheckTechnologiesAsync(command.TechnologyCodes, validation);

        validation.ThrowIfAny();

        // Save file = unique stored name 
        string storedFileName = await fileRepository.SaveAsync(command.Content, command.FileName, cancellationToken);

        try
        {
            // Describe new media item, store it as a pending change request.
            MediaSubmission submission = new()
            {
                MediaType = mediaType!.Value, // set whenever no error was reported.
                TechnologyCodes = command.TechnologyCodes.Distinct().ToList(),
                Title = command.Title!, // checked by the validator above
                Description = NullIfEmpty(command.Description),
                StoredFileName = storedFileName,
                OriginalFileName = command.FileName,
                ContentType = string.IsNullOrWhiteSpace(command.ContentType)
                    ? "application/octet-stream"
                    : command.ContentType
            };

            ChangeRequest request = new()
            {
                Target = ChangeTarget.Media,
                Action = ChangeAction.Create,
                PayloadJson = JsonSerializer.Serialize(submission, JsonOptions),
                Status = ChangeStatus.Pending,
                RequestedBy = currentUser.Id
            };

            return await changeRequestRepository.CreateAsync(request);
        }
        catch
        {
            // The request could not be created :p, so the saved file would be left behind :,. Remove it.
            await TryDeleteFileAsync(storedFileName);
            throw;
        }
    }

    // FR14: at least one technology, and every code must exist.
    // The docs call them technologies; the code and database call them machines. (meetinggg and change after)
    private async Task CheckTechnologiesAsync(List<string> codes, ValidationCollector validation)
    {
        if (codes.Count == 0)
        {
            validation.Add("Choose at least one technology.");
            return;
        }

        List<Machine> machines = await machineRepository.GetManyAsync();
        HashSet<string> known = machines.Select(machine => machine.Code).ToHashSet();

        foreach (string code in codes.Where(code => !known.Contains(code)).Distinct())
        {
            validation.Add($"Unknown technology '{code}'.");
        }
    }

    //description = three empty texts means "no description"
    private static LocalizedText? NullIfEmpty(LocalizedText? description) =>
        description is null ||
        (string.IsNullOrWhiteSpace(description.En) &&
         string.IsNullOrWhiteSpace(description.Es) &&
         string.IsNullOrWhiteSpace(description.Fr))
            ? null
            : description;

    // Best effort: if the cleanup fails too, we keep the original error, which is more useful. (research)
    private async Task TryDeleteFileAsync(string storedFileName)
    {
        try
        {
            await fileRepository.DeleteAsync(storedFileName);
        }
        catch (DataAccessException)
        {
        }
    }
}