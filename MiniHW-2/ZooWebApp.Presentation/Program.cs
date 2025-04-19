using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using ZooWebApp.Application.Services;
using ZooWebApp.Domain.Common.Interfaces;
using ZooWebApp.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://localhost:5001"); // Change port to 5001

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register application services
builder.Services.AddScoped<IAnimalService, AnimalService>();
builder.Services.AddScoped<IFeedingScheduleService, FeedingScheduleService>();
builder.Services.AddScoped<IZooStatisticsService, ZooStatisticsService>();
// Register repositories as singletons to persist data between requests
builder.Services.AddSingleton<IAnimalRepository, InMemoryAnimalRepository>();
builder.Services.AddSingleton<IEnclosureRepository, InMemoryEnclosureRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
// Always enable Swagger for this demo application
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "ZooWebApp API V1");
    options.RoutePrefix = "swagger";
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
