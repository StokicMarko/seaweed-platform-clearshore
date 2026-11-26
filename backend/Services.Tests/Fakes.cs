using Entities;
using RepositoryContracts;
using Services;

namespace Services.Tests;

public class FakeChangeRequestRepository : IChangeRequestRepository
{
    public List<ChangeRequest> Requests { get; } = [];

    // Ignores the filters on purpose, so the guard's own filtering is what the tests exercise.
    public Task<List<ChangeRequest>> GetManyAsync(ChangeTarget? targetType, int? targetId, ChangeStatus? status) =>
        Task.FromResult(Requests.ToList());

    public Task<ChangeRequest> CreateAsync(ChangeRequest request)
    {
        request.Id = Requests.Count + 100;
        Requests.Add(request);
        return Task.FromResult(request);
    }

    public Task<ChangeRequest> GetSingleAsync(int id) => throw new NotImplementedException();
    public Task<ChangeRequest> UpdateStatusAsync(int id, ChangeStatus status) => throw new NotImplementedException();
    public Task DeleteAsync(int id) => throw new NotImplementedException();
}

/// Only reads work. Any write fails the test: nothing may change before the Main Admin approves (FR3).
public class FakeMediaRepository : IMediaRepository
{
    public int ExistingId { get; set; } = 5;

    public Task<Media> GetSingleAsync(int id) =>
        id == ExistingId
            ? Task.FromResult(new Media
            {
                Id = id,
                Title = new LocalizedText("a", "b", "c"),
                OriginalFileName = "old.png",
                StoredFileName = "old-stored.png",
                ContentType = "image/png"
            })
            : throw new NotFoundException("Media not found.");

    public Task<List<Media>> GetManyAsync(string? technologyType) => throw new NotImplementedException();
    public Task CopyFileToAsync(int id, Stream destination, CancellationToken cancellationToken) => throw new NotImplementedException();
    public Task<Media> CreateAsync(MediaSubmission submission) => throw new InvalidOperationException("Media must not be created directly.");
    public Task<Media> UpdateAsync(int id, MediaSubmission submission) => throw new InvalidOperationException("Media must not be updated directly.");
    public Task DeleteAsync(int id) => throw new InvalidOperationException("Media must not be deleted directly.");
}

public class FakeFileRepository : IFileRepository
{
    public List<string> Saved { get; } = [];

    public Task<string> SaveAsync(Stream content, string originalFileName, CancellationToken cancellationToken)
    {
        string stored = $"stored-{Saved.Count}{Path.GetExtension(originalFileName)}";
        Saved.Add(stored);
        return Task.FromResult(stored);
    }

    public Task DeleteAsync(string storedFileName) => Task.CompletedTask;
    public Task CopyToAsync(string storedFileName, Stream destination, CancellationToken cancellationToken) => throw new NotImplementedException();
}

public class FakeTechnologyRepository : ITechnologyRepository
{
    public Task<List<Technology>> GetManyAsync() =>
        Task.FromResult(new List<Technology> { new() { Type = "Cultivation" }, new() { Type = "Processing" } });
}

public class FakeCurrentUser : ICurrentUser
{
    public int Id => 7;
    public UserRole Role => UserRole.Staff;
}