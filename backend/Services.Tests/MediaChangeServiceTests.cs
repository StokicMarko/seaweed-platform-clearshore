using System.Text;
using System.Text.Json;
using Entities;
using Microsoft.Extensions.Options;
using RepositoryContracts;
using Xunit;

namespace Services.Tests;

public class MediaChangeServiceTests
{
    private readonly FakeChangeRequestRepository _requests = new();
    private readonly FakeFileRepository _files = new();
    private readonly FakeMediaRepository _media = new();
    private readonly MediaChangeService _service;

    public MediaChangeServiceTests()
    {
        MediaUploadOptions options = new()
        {
            Rules = { [MediaType.Image] = new MediaFileRule { AllowedExtensions = [".png"], MaxBytes = 1_000_000 } }
        };

        _service = new MediaChangeService(
            _media,
            new FakeTechnologyRepository(),
            _files,
            new MediaFileValidator(Options.Create(options)),
            new OpenRequestGuard(_requests),
            new ChangeRequestSubmitter(_requests, _files, new FakeCurrentUser()));
    }

    private static UpdateMediaCommand ValidCommand() => new()
    {
        TechnologyTypes = ["Cultivation"],
        Title = new LocalizedText("Title", "Título", "Titre")
    };

    private static void AddOpen(FakeChangeRequestRepository requests, ChangeStatus status) =>
        requests.Requests.Add(new ChangeRequest
        {
            Id = 1, TargetType = ChangeTarget.Media, TargetId = 5, Type = ChangeType.Update, Status = status
        });

    [Theory]
    [InlineData(ChangeStatus.Pending)]
    [InlineData(ChangeStatus.Returned)]
    public async Task Update_is_blocked_by_an_open_request_and_stores_nothing(ChangeStatus status)
    {
        AddOpen(_requests, status);
        UpdateMediaCommand command = ValidCommand();
        command.Content = new MemoryStream(Encoding.UTF8.GetBytes("x"));
        command.FileName = "new.png";
        command.Length = 1;

        await Assert.ThrowsAsync<OpenRequestExistsException>(() => _service.SubmitUpdateAsync(5, command, default));

        Assert.Empty(_files.Saved);
        Assert.Single(_requests.Requests); // only the one that was already there
    }

    [Fact]
    public async Task Delete_is_blocked_by_an_open_request()
    {
        AddOpen(_requests, ChangeStatus.Pending);

        await Assert.ThrowsAsync<OpenRequestExistsException>(() => _service.SubmitDeleteAsync(5));

        Assert.Single(_requests.Requests);
    }

    [Fact]
    public async Task Unknown_item_gives_not_found()
    {
        await Assert.ThrowsAsync<NotFoundException>(() => _service.SubmitDeleteAsync(99));
    }

    [Fact]
    public async Task Update_without_a_file_keeps_the_current_file()
    {
        ChangeRequest request = await _service.SubmitUpdateAsync(5, ValidCommand(), default);

        Assert.Equal(ChangeType.Update, request.Type);
        Assert.Equal(5, request.TargetId);
        Assert.Equal(ChangeStatus.Pending, request.Status);
        Assert.Empty(_files.Saved);

        using JsonDocument payload = JsonDocument.Parse(request.PayloadJson!);
        Assert.Equal(JsonValueKind.Null, payload.RootElement.GetProperty("storedFileName").ValueKind);
        Assert.Equal(JsonValueKind.Null, payload.RootElement.GetProperty("mediaType").ValueKind);
    }

    [Fact]
    public async Task Update_with_a_file_stores_it()
    {
        UpdateMediaCommand command = ValidCommand();
        command.Content = new MemoryStream(Encoding.UTF8.GetBytes("x"));
        command.FileName = "new.png";
        command.Length = 1;
        command.ContentType = "image/png";

        ChangeRequest request = await _service.SubmitUpdateAsync(5, command, default);

        Assert.Single(_files.Saved);
        using JsonDocument payload = JsonDocument.Parse(request.PayloadJson!);
        Assert.Equal(_files.Saved[0], payload.RootElement.GetProperty("storedFileName").GetString());
        Assert.Equal("Image", payload.RootElement.GetProperty("mediaType").GetString());
    }

    [Fact]
    public async Task Update_with_an_unsupported_file_is_rejected_and_stores_nothing()
    {
        UpdateMediaCommand command = ValidCommand();
        command.Content = new MemoryStream(Encoding.UTF8.GetBytes("x"));
        command.FileName = "new.exe";
        command.Length = 1;

        await Assert.ThrowsAsync<ValidationFailedException>(() => _service.SubmitUpdateAsync(5, command, default));

        Assert.Empty(_files.Saved);
        Assert.Empty(_requests.Requests);
    }

    [Fact]
    public async Task Update_with_a_missing_language_is_rejected_and_not_filled_in()
    {
        UpdateMediaCommand command = ValidCommand();
        command.Title = new LocalizedText("Title", "Título", "");

        ValidationFailedException error =
            await Assert.ThrowsAsync<ValidationFailedException>(() => _service.SubmitUpdateAsync(5, command, default));

        Assert.Contains(error.Errors, message => message.Contains("Title"));
        Assert.Empty(_requests.Requests);
    }

    [Fact]
    public async Task Update_with_a_partly_filled_description_is_rejected()
    {
        UpdateMediaCommand command = ValidCommand();
        command.Description = new LocalizedText("Only English", "", "");

        await Assert.ThrowsAsync<ValidationFailedException>(() => _service.SubmitUpdateAsync(5, command, default));
    }

    [Fact]
    public async Task Update_with_an_empty_description_is_accepted_as_no_description()
    {
        UpdateMediaCommand command = ValidCommand();
        command.Description = new LocalizedText("", "", "");

        ChangeRequest request = await _service.SubmitUpdateAsync(5, command, default);

        using JsonDocument payload = JsonDocument.Parse(request.PayloadJson!);
        Assert.Equal(JsonValueKind.Null, payload.RootElement.GetProperty("description").ValueKind);
    }

    [Fact]
    public async Task Update_with_an_unknown_technology_is_rejected()
    {
        UpdateMediaCommand command = ValidCommand();
        command.TechnologyTypes = ["Nope"];

        await Assert.ThrowsAsync<ValidationFailedException>(() => _service.SubmitUpdateAsync(5, command, default));
    }

    [Fact]
    public async Task Delete_creates_a_pending_request_with_no_payload()
    {
        ChangeRequest request = await _service.SubmitDeleteAsync(5);

        Assert.Equal(ChangeType.Delete, request.Type);
        Assert.Equal(ChangeTarget.Media, request.TargetType);
        Assert.Equal(5, request.TargetId);
        Assert.Equal(ChangeStatus.Pending, request.Status);
        Assert.Null(request.PayloadJson);
    }

    [Fact]
    public async Task Edit_and_delete_never_write_to_the_media_item()
    {
        await _service.SubmitUpdateAsync(5, ValidCommand(), default);
        _requests.Requests.Clear();
        await _service.SubmitDeleteAsync(5);
    }
}