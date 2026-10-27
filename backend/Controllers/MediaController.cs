using System.Globalization;
using Backend.Grpc;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc;
using GrpcMediaItem = Backend.Grpc.MediaItem;
using MediaItem = Backend.Models.MediaItem;

namespace Backend.Controllers;

[ApiController]
[Route("api/media")]
public class MediaController(MediaData.MediaDataClient dataClient) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<MediaItem>>> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            var reply = await dataClient.ListMediaAsync(new ListMediaRequest(), cancellationToken: cancellationToken);
            return reply.Items.Select(ToModel).ToList();
        }
        catch (RpcException)
        {
            return StatusCode(StatusCodes.Status502BadGateway);
        }
    }

    private static MediaItem ToModel(GrpcMediaItem item) => new(
        item.Id,
        NullIfEmpty(item.Machine),
        item.TitleEn,
        item.TitleFr,
        item.TitleEs,
        NullIfEmpty(item.DescriptionEn),
        NullIfEmpty(item.DescriptionFr),
        NullIfEmpty(item.DescriptionEs),
        item.FileName,
        item.Url,
        item.ContentType,
        DateTimeOffset.Parse(item.CreatedAt, CultureInfo.InvariantCulture));

    private static string? NullIfEmpty(string value) => value.Length == 0 ? null : value;
}
