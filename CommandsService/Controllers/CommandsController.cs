using AutoMapper;
using CommandsService.Data;
using CommandsService.DTOs;
using CommandsService.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers;

[ApiController]
[Route("api/c/platforms/{platformId}/[controller]")]
public class CommandsController : ControllerBase
{
    private readonly ICommandRepo _repository;
    private readonly IMapper _mapper;

    public CommandsController(ICommandRepo repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    [HttpGet]
    public ActionResult<IEnumerable<CommandReadDto>> GetCommandsForPlatform(int platformId)
    {
        Console.WriteLine($"Hit gercommandsforplatform: {platformId}");
        if(!_repository.PlatformExist(platformId))
        {
            return NotFound();
        }
        var commands = _repository.GetCommandsForPlatform(platformId);
        return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(commands));
    }

    [HttpGet("{commandId}",Name = "GetCommandForPlatform")]
    public ActionResult<CommandReadDto> GetCommandForPlatform(int platformId, int commandId)
    {
        Console.WriteLine($"Hit gercommandforplatform: {platformId}/{commandId}");
        if(!_repository.PlatformExist(platformId))
        {
            return NotFound();
        }
        var command = _repository.GetCommand(platformId,commandId);   
        if(command == null)
        {
            return NotFound();
        }
        return Ok(_mapper.Map<CommandReadDto>(command));
    }

    [HttpPost]
    public ActionResult<CommandReadDto> CreateCommandForPlatform(int platformId, CommandCreateDto commandDto)
    {
        Console.WriteLine($"Hit CreateCommandForPlatform: {platformId}");
        if(!_repository.PlatformExist(platformId))
        {
            return NotFound();
        }

        var command = _mapper.Map<Command>(commandDto);
        _repository.CreateCommand(platformId,command);
        _repository.SaveChanges();

        var commandreadDto = _mapper.Map<CommandReadDto>(command);
        return CreatedAtRoute(nameof(GetCommandForPlatform),
            new{platformId = platformId, commandId = commandreadDto.Id},commandreadDto);
    }
}
