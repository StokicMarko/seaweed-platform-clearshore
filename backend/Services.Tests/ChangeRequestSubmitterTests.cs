using Entities;
using Xunit;

namespace Services.Tests;

public class ChangeRequestSubmitterTests
{
    [Fact]
    public async Task Stores_the_target_id_and_the_current_user()
    {
        FakeChangeRequestRepository requests = new();
        ChangeRequestSubmitter submitter = new(requests, new FakeFileRepository(), new FakeCurrentUser());

        ChangeRequest request = await submitter.SubmitAsync<string>(ChangeTarget.Media, ChangeType.Delete, 5, null, []);

        Assert.Equal(5, request.TargetId);
        Assert.Equal(7, request.RequestedBy);
        Assert.Null(request.PayloadJson);
    }
}