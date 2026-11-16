namespace Services;

/// Filled from the "CurrentUser" section of appsettings.
public class CurrentUserOptions
{
    public const string SectionName = "CurrentUser";

    public int Id { get; set; }
}