namespace RepositoryContracts;

/// Saves and deletes uploaded files on the data server.
///FOR ANY feature that stores a file (media, news, country photo) pleaseee solid people

public interface IFileRepository
{
    /// Streams a file to the data server and returns the stored file name it chose.
    /// Streaming keeps large videos out of memory --- chunckss
    
    Task<string> SaveAsync(Stream content, string originalFileName, CancellationToken cancellationToken);
    
    /// Deletes a stored file.
    /// When a request fails after the upload, and later when a request is declined.
    Task DeleteAsync(string storedFileName);

    /// Streams a stored file to the destination, found by its stored name.
    /// For files that have no media item yet, like a pending upload in the admin review.
    Task CopyToAsync(string storedFileName, Stream destination, CancellationToken cancellationToken);
}