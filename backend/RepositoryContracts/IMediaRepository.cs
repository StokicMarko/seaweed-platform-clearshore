using Entities;

namespace RepositoryContracts;

public interface IMediaRepository
{
    //Async-Taks -> Waiting for the network call to tier 3
    Task<List<Media>> GetManyAsync(string? technologyType);
    Task<Media> GetSingleAsync(int id);
    Task CopyFileToAsync(int id, Stream destination, CancellationToken cancellationToken);
    
    //Creates the media item from full data, uses stored file name
    Task<Media> CreateAsync(MediaSubmission submission);

    /// Replaces with full data. 
    Task<Media> UpdateAsync(int id, MediaSubmission submission);

    /// Deletes= media item row only. Stored file is deleted through IFileRepository.
    Task DeleteAsync(int id);
}