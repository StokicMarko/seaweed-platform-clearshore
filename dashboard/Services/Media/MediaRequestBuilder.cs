using System.Net.Http.Headers;
using Dashboard.Models;
using Microsoft.AspNetCore.Components.Forms;

namespace Dashboard.Services.Media;

public static class MediaRequestBuilder
{
    public static MultipartFormDataContent Build(
        MediaFormData form,
        IBrowserFile? file,
        Stream? stream)
    {
        MultipartFormDataContent multipart = new();
        if (file is not null && stream is not null)
        {
            StreamContent fileContent = new(stream);

            fileContent.Headers.ContentType =
                MediaTypeHeaderValue.TryParse(
                    file.ContentType,
                    out var parsedContentType)
                    ? parsedContentType
                    : new MediaTypeHeaderValue(
                        "application/octet-stream");

            multipart.Add(
                fileContent,
                "File",
                Path.GetFileName(file.Name));
        }

        foreach (string type in form.TechnologyTypes)
        {
            multipart.Add(new StringContent(type), "TechnologyTypes");
        }

        multipart.Add(new StringContent(form.TitleEn.Trim()), "TitleEn");
        multipart.Add(new StringContent(form.TitleEs.Trim()), "TitleEs");
        multipart.Add(new StringContent(form.TitleFr.Trim()), "TitleFr");

        multipart.Add(
            new StringContent(form.DescriptionEn.Trim()),
            "DescriptionEn");
        multipart.Add(
            new StringContent(form.DescriptionEs.Trim()),
            "DescriptionEs");
        multipart.Add(
            new StringContent(form.DescriptionFr.Trim()),
            "DescriptionFr");

        return multipart;
    }
}