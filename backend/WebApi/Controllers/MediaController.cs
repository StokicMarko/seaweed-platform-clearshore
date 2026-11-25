using ApiContracts.Media;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;
using WebApi.Mappers;
using ApiContracts.ChangeRequests;
using Services;
using WebApi.Requests;

namespace WebApi.Controllers;

[ApiController]
[Route("api/media")]
public class MediaController(
    IMediaRepository mediaRepository,
    IMediaSubmissionService submissionService,
    IMediaChangeService changeService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<MediaDto>>> GetMany([FromQuery] string? technology)
    {
        List<Media> media = await mediaRepository.GetManyAsync(technology);
        return media.Select(DtoMapper.ToDto).ToList();
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<MediaDto>> GetSingle([FromRoute] int id)
    {
        Media media = await mediaRepository.GetSingleAsync(id);
        return DtoMapper.ToDto(media);
    }

    [HttpGet("{id:int}/file")]
    public async Task GetFile([FromRoute] int id)
    {
        Media media = await mediaRepository.GetSingleAsync(id);
        Response.ContentType = media.ContentType;
        await mediaRepository.CopyFileToAsync(id, Response.Body, HttpContext.RequestAborted);
    }


    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<ChangeRequestDto>> Add([FromForm] AddMediaForm form,
        CancellationToken cancellationToken)
    {
        if (form.File is null)
        {
            throw new ValidationFailedException(["Choose a file to upload."]);
        }

        await using Stream content = form.File.OpenReadStream();
        ChangeRequest request = await submissionService.SubmitAsync(
            AddMediaMapper.ToCommand(form, form.File, content), cancellationToken);

        return Accepted(DtoMapper.ToDto(request));
    }

    [HttpGet("{id:int}/change-availability")]
    public async Task<IActionResult> CheckChangeAvailability([FromRoute] int id)
    {
        await changeService.EnsureCanChangeAsync(id);
        return Ok();
    }

    [HttpPut("{id:int}")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<ChangeRequestDto>> Update(
        [FromRoute] int id, [FromForm] UpdateMediaForm form, CancellationToken cancellationToken)
    {
        Stream? content = form.File?.OpenReadStream();
        try
        {
            ChangeRequest request = await changeService.SubmitUpdateAsync(
                id, UpdateMediaMapper.ToCommand(form, content), cancellationToken);
            return Accepted(DtoMapper.ToDto(request));
        }
        finally
        {
            if (content is not null)
            {
                await content.DisposeAsync();
            }
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ChangeRequestDto>> Delete([FromRoute] int id)
    {
        ChangeRequest request = await changeService.SubmitDeleteAsync(id);
        return Accepted(DtoMapper.ToDto(request));
    }
}