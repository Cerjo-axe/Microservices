using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.AsyncDataServices;
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
    private readonly IMessageBusClient _messageBusClient;

    public PlatformsController(
        IPlatformRepo repository, 
        IMapper mapper,
        ICommandDataClient commandDataClient,
        IMessageBusClient messagebusClient)
    {
        _repository = repository;
        _mapper = mapper;
        _commandDataClient = commandDataClient;
        _messageBusClient = messagebusClient;
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

        //Send Sync Message
        try
        {
            await _commandDataClient.SendPlatformToCommand(platformReadDTO);
        }
        catch(Exception ex)
        {
            Console.WriteLine($"Could nos send synchronously: {ex}");
        }

        //Send async message
        try
        {
            var platformPublishedDto = _mapper.Map<PlatformPublishedDto>(platformReadDTO);
            platformPublishedDto.Event="Platform_Published";
            _messageBusClient.PublishNewPlatform(platformPublishedDto);
        }
        catch (Exception ex)
        {
            
            Console.WriteLine($"Could nos send asynchronously: {ex}");
        }


        return CreatedAtRoute(nameof(GetPlatformById), new {Id = platformReadDTO.Id},platformReadDTO);
    }
}
