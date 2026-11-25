using ApiContracts.ChangeRequests;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Services;
using WebApi.Mappers;

namespace WebApi.Controllers;

///tier 2 front door, 
/// Receives the dASHBOARD call and hands it to the service.
///The Main Admin's review API (UC09 to UC12).

[ApiController]
[Route("api/change-requests")]
public class ChangeRequestsController(IChangeRequestReviewService reviewService) : ControllerBase
{
    // UC09: list of the pending requests
    [HttpGet("pending")]
    public async Task<ActionResult<List<ChangeRequestDto>>> GetPending()
    {
        List<ChangeRequest> requests = await reviewService.GetPendingAsync();
        return requests.Select(DtoMapper.ToDto).ToList();
    }

    // UC09: open one request, with the data it proposes (the preview)
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ChangeRequestDetailDto>> GetSingle([FromRoute] int id)
    {
        ChangeRequest request = await reviewService.GetSingleAsync(id);
        return DtoMapper.ToDetailDto(request);
    }

    // UC09: preview the uploaded file of a pending request (photo, video or 3D model), by its stored name.
    [HttpGet("{id:int}/files/{storedFileName}")]
    public async Task GetFile([FromRoute] int id, [FromRoute] string storedFileName)
    {
        // First the checks (admin, pending, file belongs to the request). They also give the content type.
        RequestFile file = await reviewService.GetFileAsync(id, storedFileName);
        Response.ContentType = file.ContentType;

        // Then stream the file into the response.
        await reviewService.CopyFileToAsync(id, storedFileName, Response.Body, HttpContext.RequestAborted);
    } 
    
    // UC10: approve. Nothing to send back, the request is gone, so 204 No Content.
    [HttpPost("{id:int}/approve")]
    public async Task<IActionResult> Approve([FromRoute] int id)
    {
        await reviewService.ApproveAsync(id);
        return NoContent();
    }

    // UC11: decline. The request and its files are erased, so 204 No Content.
    [HttpPost("{id:int}/decline")]
    public async Task<IActionResult> Decline([FromRoute] int id)
    {
        await reviewService.DeclineAsync(id);
        return NoContent();
    }

    // UC12: return. The request stays with status Returned, so we send it back.
    [HttpPost("{id:int}/return")]
    public async Task<ActionResult<ChangeRequestDto>> Return([FromRoute] int id)
    {
        ChangeRequest request = await reviewService.ReturnAsync(id);
        return DtoMapper.ToDto(request);
    }
}