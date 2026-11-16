using ApiContracts.Media;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;
using WebApi.Mappers;

namespace WebApi.Controllers;

[ApiController]
[Route("api/media")]
public class MediaController(IMediaRepository mediaRepository) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<MediaDto>>> GetMany([FromQuery] string? machine)
    {
        List<Media> media = await mediaRepository.GetManyAsync(machine);
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
}
