using Microsoft.EntityFrameworkCore;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.SyncDataServices.Grpc;
using PlatformService.SyncDataServices.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

if(builder.Environment.IsProduction())
{
    Console.WriteLine("Using sql server");
    builder.Services.AddDbContext<AppDbContext>(opt=>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("PlatformsConn")));
}else
{
    Console.WriteLine("Using inmem db");
    builder.Services.AddDbContext<AppDbContext>(opt=>
    opt.UseInMemoryDatabase("InMem"));
}


builder.Services.AddScoped<IPlatformRepo,PlatformRepo>();

builder.Services.AddHttpClient<ICommandDataClient,HttpCommandDataClient>();
builder.Services.AddSingleton<IMessageBusClient,MessageBusClient>();
builder.Services.AddGrpc();
builder.Services.AddControllers();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints=>
{
    endpoints.MapControllers();
    endpoints.MapGrpcService<GrpcPlatformService>();

    endpoints.MapGet("/protos/platforms.proto", async context =>
    {
        await context.Response.WriteAsync(File.ReadAllText("Protos/platforms.proto"));
    });
});
PrepDb.PrepPopulation(app, builder.Environment.IsProduction());

app.Run();
