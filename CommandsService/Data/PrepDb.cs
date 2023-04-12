using CommandsService.Models;
using CommandsService.SyncDataServices.Grpc;

namespace CommandsService.Data;

public static class PrepDb
{
    public static void PrepPopulation(IApplicationBuilder appBuilder)
    {
        using(var serviceScope = appBuilder.ApplicationServices.CreateScope())
        {
            var grpcClient = serviceScope.ServiceProvider.GetService<IPlatformDataClient>();

            var platforms = grpcClient.ReturnAllPlatforms();

            SeedData(serviceScope.ServiceProvider.GetService<ICommandRepo>(),platforms);
        }
    }

    private static void SeedData(ICommandRepo repo, IEnumerable<Platform> platforms)
    {
        Console.WriteLine("Seeding new Platforms");

        foreach (var plat in platforms)
        {
            if(!repo.ExternalPlatformExist(plat.ExternalId))
            {
                repo.CreatePlatform(plat);
            }
            repo.SaveChanges();
        }
    }
}
