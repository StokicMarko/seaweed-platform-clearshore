using ApiContracts.Media;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Services;

namespace WebApi.Controllers;

[ApiController]
[Route("api/media/upload-rules")]
public class MediaUploadController(
    IOptions<MediaUploadOptions> options) : ControllerBase
{
    [HttpGet]
    public ActionResult<List<MediaUploadRuleDto>> GetRules()
    {
        return options.Value.Rules
            .Select(entry => new MediaUploadRuleDto
            {
                MediaType = entry.Key.ToString(),
                AllowedExtensions = entry.Value.AllowedExtensions,
                MaxBytes = entry.Value.MaxBytes
            })
            .ToList();
    }
}