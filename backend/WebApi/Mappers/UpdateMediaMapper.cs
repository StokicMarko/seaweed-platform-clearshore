using Entities;
using Services;
using WebApi.Requests;

namespace WebApi.Mappers;

public static class UpdateMediaMapper
{
    /// content is the opened stream of form.File, or null when no file was sent.
    public static UpdateMediaCommand ToCommand(UpdateMediaForm form, Stream? content) => new()
    {
        Content = content,
        FileName = form.File?.FileName,
        Length = form.File?.Length ?? 0,
        ContentType = form.File?.ContentType,
        TechnologyTypes = form.TechnologyTypes,
        Title = new LocalizedText(form.TitleEn ?? "", form.TitleEs ?? "", form.TitleFr ?? ""),
        Description = new LocalizedText(form.DescriptionEn ?? "", form.DescriptionEs ?? "", form.DescriptionFr ?? "")
    };
}