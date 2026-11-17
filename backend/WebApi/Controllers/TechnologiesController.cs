using ApiContracts.Technology;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;
using WebApi.Mappers;

namespace WebApi.Controllers;

[ApiController]
[Route("api/technologies")]
public class TechnologiesController(ITechnologyRepository technologyRepository) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<TechnologyDto>>> GetMany()
    {
        List<Technology> technologies = await technologyRepository.GetManyAsync();
        return technologies.Select(DtoMapper.ToDto).ToList();
    }
}
