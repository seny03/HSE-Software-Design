using AnalysisService.Data;
using AnalysisService.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://0.0.0.0:8082");

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/analysis-service-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// Configure database connection
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AnalysisDbContext>(options =>
    options.UseNpgsql(connectionString));

// Register HTTP clients
var fileServiceUrl = builder.Configuration["FileServiceUrl"] ?? "http://file-service:8081";
builder.Services.AddHttpClient("FileService", client =>
{
    client.BaseAddress = new Uri(fileServiceUrl);
});

// Register custom services
builder.Services.AddScoped<ITextAnalysisService, TextAnalysisService>();
builder.Services.AddScoped<IFileClientService, FileClientService>();
builder.Services.AddScoped<IWordCloudService, WordCloudService>();

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
        Title = "Analysis Service API",
        Version = "v1",
        Description = "Text Analysis Service for Text Scanner application"
    });
});

var app = builder.Build();

// Configure middleware
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Analysis Service API v1"));
}

app.UseSerilogRequestLogging();
app.UseRouting();
app.UseCors();
app.UseAuthorization();
app.MapControllers();

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AnalysisDbContext>();
    dbContext.Database.Migrate();
}

// Ensure directories exist
Directory.CreateDirectory("logs");

app.Run();
