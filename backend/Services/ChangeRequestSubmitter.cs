using System.Text.Json;
using Entities;
using RepositoryContracts;

namespace Services;

public class ChangeRequestSubmitter(
    IChangeRequestRepository changeRequestRepository,
    IFileRepository fileRepository,
    ICurrentUser currentUser) : IChangeRequestSubmitter
{
    public async Task<ChangeRequest> SubmitAsync<TPayload>(
        ChangeTarget target,
        ChangeType type,
        TPayload payload,
        IReadOnlyCollection<string> storedFileNames)
    {
        try
        {
            ChangeRequest request = new()
            {
                TargetType = target,
                Type = type,
                PayloadJson = JsonSerializer.Serialize(payload, ChangeRequestJson.Options),
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