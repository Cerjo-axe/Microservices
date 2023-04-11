using PlatformService.DTOs;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace PlatformService.SyncDataServices.Http;

public class HttpCommandDataClient : ICommandDataClient
{
    private readonly HttpClient _client;
    private readonly IConfiguration _configuration;

    public HttpCommandDataClient(HttpClient client, IConfiguration configuration)
    {
        _client = client;
        _configuration = configuration;
    }
    public async Task SendPlatformToCommand(PlatformReadDTO plat)
    {
        var httpContent = new StringContent(
            JsonSerializer.Serialize(plat),
            Encoding.UTF8,
            "application/json"
        );

        var response =await _client.PostAsync($"{_configuration["CommandService"]}",httpContent);
        if(response.IsSuccessStatusCode)
        {
            Console.WriteLine("sync POST to CommandService was OK!");
        }else
        {
            Console.WriteLine("Sync post to commandservice was not ok");
        }
    }
}
