using System.Text.Json;
using System.Text.Json.Serialization;
using Entities;
using RepositoryContracts;

namespace Services;

public class ChangeRequestSubmitter(
    IChangeRequestRepository changeRequestRepository,
    IFileRepository fileRepository,
    ICurrentUser currentUser) : IChangeRequestSubmitter
{
    // The payload is stored as JSON text and the Java side reads it later.
    // Enums are written as names (for example "Video").
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    public async Task<ChangeRequest> SubmitAsync<TPayload>(
        ChangeTarget target,
        ChangeType type,
        int? targetId,
        TPayload? payload,
        IReadOnlyCollection<string> storedFileNames)
    {
        try
        {
            ChangeRequest request = new()
            {
                TargetType = target,
                Type = type,
                TargetId = targetId,
                PayloadJson = payload is null ? null : JsonSerializer.Serialize(payload, JsonOptions),
                Status = ChangeStatus.Pending,
                RequestedBy = currentUser.Id
            };

            return await changeRequestRepository.CreateAsync(request);
        }
        catch
        {
            // The request was not saved, so the stored files would be left behind. Remove them.
            await TryDeleteFilesAsync(storedFileNames);
            throw;
        }
    }

    // If the cleanup fails too, we keep the original error (most usefull)
    private async Task TryDeleteFilesAsync(IEnumerable<string> storedFileNames)
    {
        foreach (string storedFileName in storedFileNames)
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
}