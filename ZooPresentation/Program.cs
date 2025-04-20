

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

// Application interfaces
using ZooApplication.Interfaces.Repositories;
using ZooApplication.Interfaces.Services;
// Application implementations
using ZooApplication.Services;
using ZooInfrastructure;
using ZooInfrastructure.InMemory;

var builder = WebApplication.CreateBuilder(args);

// 1) Infrastructure
builder.Services.AddSingleton<IAnimalRepository, InMemoryAnimalRepository>();
builder.Services.AddSingleton<IEnclosureRepository, InMemoryEnclosureRepository>();
builder.Services.AddSingleton<IFeedingScheduleRepository, InMemoryFeedingScheduleRepository>();
builder.Services.AddScoped<IEventDispatcher, EventDispatcher>();

// 2) Application
builder.Services.AddScoped<IAnimalTransferService, AnimalTransferService>();
builder.Services.AddScoped<IFeedingOrganizationService, FeedingOrganizationService>();
builder.Services.AddScoped<IZooStatisticsService, ZooStatisticsService>();

// 3) Controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Zoo API",
        Version = "v1",
        Description = "REST API for Zoo Management"
    });
});

var app = builder.Build();

// 4) Swagger UI (в режиме Development)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Zoo API v1");
    });
}

// 5) Маршруты
app.MapControllers();

// 6) Редирект корня на Swagger
app.MapGet("/", () => Results.Redirect("/swagger/index.html"));

// 7) Запуск
app.Run();
