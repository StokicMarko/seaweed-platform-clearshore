using ApiContracts.Machine;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;
using WebApi.Mappers;

namespace WebApi.Controllers;

[ApiController]
[Route("api/machines")]
public class MachinesController(IMachineRepository machineRepository) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<MachineDto>>> GetMany()
    {
        List<Machine> machines = await machineRepository.GetManyAsync();
        return machines.Select(DtoMapper.ToDto).ToList();
    }
}
