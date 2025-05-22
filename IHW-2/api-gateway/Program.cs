using ApiGateway.Services;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/api-gateway-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// Configure service URLs from environment variables
var fileServiceUrl = builder.Configuration["FileServiceUrl"] ?? "http://file-service:8081";
var analysisServiceUrl = builder.Configuration["AnalysisServiceUrl"] ?? "http://analysis-service:8082";

// Register HTTP clients for services
builder.Services.AddHttpClient("FileService", client =>
{
    client.BaseAddress = new Uri(fileServiceUrl);
});

builder.Services.AddHttpClient("AnalysisService", client =>
{
    client.BaseAddress = new Uri(analysisServiceUrl);
});

// Register custom services
builder.Services.AddSingleton<IFileServiceClient, FileServiceClient>();
builder.Services.AddSingleton<IAnalysisServiceClient, AnalysisServiceClient>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Text Scanner API Gateway",
        Version = "v1",
        Description = "API Gateway for Text Scanner application"
    });
});

var app = builder.Build();

// Configure middleware
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Text Scanner API Gateway v1"));
}

app.UseSerilogRequestLogging();
app.UseRouting();
app.UseCors();
app.UseAuthorization();
app.MapControllers();

// Ensure logs directory exists
Directory.CreateDirectory("logs");

app.Run();
