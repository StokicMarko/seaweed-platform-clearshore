using Entities;
using RepositoryContracts;

namespace Services;

/// Media rules shared by Add (UC03) and Edit (UC04).
public static class MediaSubmissionRules
{
    /// At least one technology, and every type must exist.
    public static async Task CheckTechnologiesAsync(
        ITechnologyRepository technologyRepository, List<string> types, ValidationCollector validation)
    {
        if (types.Count == 0)
        {
            validation.Add("Choose at least one technology.");
            return;
        }

        List<Technology> technologies = await technologyRepository.GetManyAsync();
        HashSet<string> known = technologies.Select(technology => technology.Type).ToHashSet();

        foreach (string type in types.Where(type => !known.Contains(type)).Distinct())
        {
            validation.Add($"Unknown technology '{type}'.");
        }
    }

    /// Three empty texts mean "no description".
    public static LocalizedText? NullIfEmpty(LocalizedText? description) =>
        description is null ||
        (string.IsNullOrWhiteSpace(description.En) &&
         string.IsNullOrWhiteSpace(description.Es) &&
         string.IsNullOrWhiteSpace(description.Fr))
            ? null
            : description;
}