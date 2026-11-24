using Entities;
using RepositoryContracts;

namespace Services;

//interface lists, this  code behind it as admin power
//Main Admin's review work: list, open, approve, decline and return change requests.
/// Business rules - who may act, and only on pending requests.
/// Works for every target type, the applier registry picks the right applier (open-closed) SOLIDDDDD .
public class ChangeRequestReviewService(
    IChangeRequestRepository changeRequestRepository,   // stored change requests (gRPC behind the interface)
    IChangeRequestApplierRegistry applierRegistry,       // finds the applier for a target type
    IFileRepository fileRepository,                      // reads the stored files for preview
    ICurrentUser currentUser) : IChangeRequestReviewService   // who is asking
{
    public async Task<List<ChangeRequest>> GetPendingAsync()
    {
        RequireMainAdmin();
        return await changeRequestRepository.GetManyAsync(null, null, ChangeStatus.Pending);  
    }

    public async Task<ChangeRequest> GetSingleAsync(int id)
    {
        RequireMainAdmin();
        return await changeRequestRepository.GetSingleAsync(id);   // unknown id: NotFoundException (404)
    }

    public async Task ApproveAsync(int id)
    {
        RequireMainAdmin();
        ChangeRequest request = await GetPendingRequestAsync(id);

        // 1. Apply the change. If this fails the request stays, so the admin can try again.
        IChangeRequestApplier applier = applierRegistry.GetFor(request.TargetType);
        await applier.ApplyAsync(request, CancellationToken.None);   
        
        // 2. Only now delete the request. If this fails, approving again is safe (the applier repeats safely).
        await changeRequestRepository.DeleteAsync(id);
    }

    public async Task DeclineAsync(int id)
    {
        RequireMainAdmin();
        ChangeRequest request = await GetPendingRequestAsync(id);

        // 1. Delete the files stored  first. If step 2 fails, declining again finishes the job.
        IChangeRequestApplier applier = applierRegistry.GetFor(request.TargetType);
        await applier.CleanupAsync(request, CancellationToken.None);

        // 2. Erase the request
        await changeRequestRepository.DeleteAsync(id);
    }

    public async Task<ChangeRequest> ReturnAsync(int id)
    {
        RequireMainAdmin();
        await GetPendingRequestAsync(id);   // only to check it is pending

        // Only the status changes. The staff member can fix and resubmit later.
        return await changeRequestRepository.UpdateStatusAsync(id, ChangeStatus.Returned);
    }
    
    public async Task<RequestFile> GetFileAsync(int id, string storedFileName)
    {
        RequireMainAdmin();
        ChangeRequest request = await GetPendingRequestAsync(id);

        // Only a file the applier says belongs to this request can be opened.
        IChangeRequestApplier applier = applierRegistry.GetFor(request.TargetType);
        RequestFile? file = applier.GetFiles(request).FirstOrDefault(f => f.StoredFileName == storedFileName);//give me the first entry that matches, or nothing if there is none

        return file ?? throw new NotFoundException("That file does not belong to this request.");
    }

    public async Task CopyFileToAsync(int id, string storedFileName, Stream destination,
        CancellationToken cancellationToken)
    {
        //can never start for a file that is not allowed.
        RequestFile file = await GetFileAsync(id, storedFileName);
        await fileRepository.CopyToAsync(file.StoredFileName, destination, cancellationToken);
    }

    

    // Rule: only the Main Admin reviews. Checked first
    private void RequireMainAdmin()
    {
        if (currentUser.Role != UserRole.MainAdmin)
        {
            throw new NotAllowedException("Only the Main Admin can review change requests.");
        }
    }

    // Rule: approve, decline and return only work on pending requests.
    private async Task<ChangeRequest> GetPendingRequestAsync(int id)
    {
        ChangeRequest request = await changeRequestRepository.GetSingleAsync(id);

        if (request.Status != ChangeStatus.Pending)
        {
            throw new ValidationFailedException(["Only a pending request can be reviewed."]);
        }

        return request;
    }
}
  
    
