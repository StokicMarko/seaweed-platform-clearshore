namespace Dashboard.Models;

public record MediaItem(
    int Id,
    string? Machine,
    string TitleEn,
    string TitleFr,
    string TitleEs,
    string? DescriptionEn,
    string? DescriptionFr,
    string? DescriptionEs,
    string FileName,
    string Url,
    string ContentType,
    DateTimeOffset CreatedAt);
