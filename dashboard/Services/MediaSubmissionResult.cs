using ApiContracts.ChangeRequests;

namespace Dashboard.Services;

public class MediaSubmissionResult
{
    public ChangeRequestDto? Request { get; set; }

    public List<string> Errors { get; set; } = new();
}