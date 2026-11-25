using Entities;
using Xunit;

namespace Services.Tests;

public class OpenRequestGuardTests
{
    private readonly FakeChangeRequestRepository _requests = new();
    private OpenRequestGuard Guard => new(_requests);

    private void Add(ChangeTarget target, int? targetId, ChangeStatus status) =>
        _requests.Requests.Add(new ChangeRequest
        {
            Id = 1, TargetType = target, TargetId = targetId, Type = ChangeType.Update, Status = status
        });

    [Theory]
    [InlineData(ChangeStatus.Pending)]
    [InlineData(ChangeStatus.Returned)]
    public async Task Blocks_when_the_item_has_an_open_request(ChangeStatus status)
    {
        Add(ChangeTarget.Media, 5, status);

        OpenRequestExistsException error = await Assert.ThrowsAsync<OpenRequestExistsException>(
            () => Guard.EnsureNoOpenRequestAsync(ChangeTarget.Media, 5));

        Assert.Equal(status, error.Status);
    }

    [Fact]
    public async Task Passes_when_there_are_no_requests()
    {
        await Guard.EnsureNoOpenRequestAsync(ChangeTarget.Media, 5);
    }

    [Fact]
    public async Task Ignores_requests_for_another_item()
    {
        Add(ChangeTarget.Media, 6, ChangeStatus.Pending);
        await Guard.EnsureNoOpenRequestAsync(ChangeTarget.Media, 5);
    }

    [Fact]
    public async Task Ignores_requests_for_another_target_type()
    {
        Add(ChangeTarget.News, 5, ChangeStatus.Pending);
        await Guard.EnsureNoOpenRequestAsync(ChangeTarget.Media, 5);
    }

    [Fact]
    public async Task Ignores_create_requests_that_have_no_target_id()
    {
        Add(ChangeTarget.Media, null, ChangeStatus.Pending);
        await Guard.EnsureNoOpenRequestAsync(ChangeTarget.Media, 5);
    }
}