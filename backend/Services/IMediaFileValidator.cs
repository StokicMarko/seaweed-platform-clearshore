using Entities;

namespace Services;

/// Checks an uploaded file against the upload rules and works out its media type.
public interface IMediaFileValidator
{
    /// Returns the media type when the file is accepted.
    /// Otherwise adds a message to validation and returns null.
    MediaType? Check(string fileName, long length, ValidationCollector validation);
}