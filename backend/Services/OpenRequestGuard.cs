using Entities;
using RepositoryContracts;

namespace Services;

public class OpenRequestGuard(IChangeRequestRepository changeRequestRepository) : IOpenRequestGuard
{
    public async Task EnsureNoOpenRequestAsync(ChangeTarget target, int targetId)
    {
        List<ChangeRequest> requests = await changeRequestRepository.GetManyAsync(target, targetId, null);

        // Filtered again here so the rule does not depend on the repository applying the filters.
        ChangeRequest? open = requests.FirstOrDefault(request =>
            request.TargetType == target &&
            request.TargetId == targetId &&
            request.Status is ChangeStatus.Pending or ChangeStatus.Returned);

        if (open is not null)
        {
            throw new OpenRequestExistsException(open.Id, open.Status);
        }
    }
}