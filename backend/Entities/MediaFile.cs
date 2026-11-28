namespace Entities;

/// One attached file: its name on the data server, its uploaded name and its type.
public record MediaFile(string StoredFileName, string OriginalFileName, string ContentType);