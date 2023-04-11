using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Data;
using PlatformService.DTOs;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;

namespace PlatformService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlatformsController : ControllerBase
{
    private readonly IPlatformRepo _repository;
    private readonly IMapper _mapper;
    private readonly ICommandDataClient _commandDataClient;

    public PlatformsController(
        IPlatformRepo repository, 
        IMapper mapper,
        ICommandDataClient commandDataClient)
    {
        _repository = repository;
        _mapper = mapper;
        _commandDataClient = commandDataClient;
    }

    [HttpGet]
    public ActionResult<IEnumerable<PlatformReadDTO>> GetPlatforms()
    {
        Console.WriteLine("Getting Platforms . . . ");

        var platformitems = _repository.GetAllPlatforms();

        return Ok(_mapper.Map<IEnumerable<PlatformReadDTO>>(platformitems));
    }

    [HttpGet("{id:int}", Name = "GetPlatformById")]
    public ActionResult<PlatformReadDTO> GetPlatformById(int id)
    {
        var platformitem = _repository.GetPlatformById(id);
        if(platformitem != null)
        {
            return Ok(_mapper.Map<PlatformReadDTO>(platformitem));
        }
        return NotFound();
    }

    [HttpPost]
    public async Task<ActionResult<PlatformReadDTO>> CreatePlatform(PlatformCreateDTO platformCreatedto)
    {
        var platformModel = _mapper.Map<Platform>(platformCreatedto);
        _repository.CreatePlatform(platformModel);
        _repository.SaveChanges();
        var platformReadDTO = _mapper.Map<PlatformReadDTO>(platformModel);
        try
        {
            await _commandDataClient.SendPlatformToCommand(platformReadDTO);
        }
        catch(Exception ex)
        {
            Console.WriteLine($"Could nos send synchronously: {ex}");
        }
        return CreatedAtRoute(nameof(GetPlatformById), new {Id = platformReadDTO.Id},platformReadDTO);
    }
}
