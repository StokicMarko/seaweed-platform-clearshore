using Entities;
using Services;
using WebApi.Requests;

namespace WebApi.Mappers;

public static class AddMediaMapper
{
    /// The stream is passed in, so the controller decides when it is opened and closed.
    public static AddMediaCommand ToCommand(AddMediaForm form, IFormFile file, Stream content) => new()
    {
        Content = content,
        FileName = file.FileName,
        Length = file.Length,
        ContentType = file.ContentType,
        TechnologyCodes = form.TechnologyCodes,
        Title = new LocalizedText(form.TitleEn ?? "", form.TitleEs ?? "", form.TitleFr ?? ""),
        Description = new LocalizedText(form.DescriptionEn ?? "", form.DescriptionEs ?? "", form.DescriptionFr ?? "")
    };
}