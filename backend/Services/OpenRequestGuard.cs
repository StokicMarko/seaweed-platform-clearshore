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
    
    public async Task<List<ChangeRequest>> GetOpenAsync(ChangeTarget target)
    {
        List<ChangeRequest> requests = await changeRequestRepository.GetManyAsync(target, null, null);
        //filtered - rule not dependant on repo applying the filters
        // request with no targetid is create
        return requests
            .Where(request => request.TargetType == target && request.TargetId is not null && IsOpen(request))
            .ToList();
    }

    //"open": still waiting (Pending) or sent back to the staff member (Returned).
    private static bool IsOpen(ChangeRequest request) =>
        request.Status is ChangeStatus.Pending or ChangeStatus.Returned;
}