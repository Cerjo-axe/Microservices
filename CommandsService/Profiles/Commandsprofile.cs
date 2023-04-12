using AutoMapper;
using CommandsService.DTOs;
using CommandsService.Models;

namespace CommandsService.Profiles;

public class Commandsprofile: Profile
{
    public Commandsprofile()
    {
        CreateMap<Platform, PlatformReadDto>();
        CreateMap<CommandCreateDto,Command>();
        CreateMap<Command,CommandReadDto>();
        CreateMap<PlatformPublishedDto, Platform>()
            .ForMember(dest=> dest.ExternalId, opt=> opt.MapFrom(src=>src.Id));
    }
}
