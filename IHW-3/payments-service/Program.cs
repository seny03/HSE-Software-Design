using Microsoft.EntityFrameworkCore;
using PaymentsService.Data;
using PaymentsService.Services;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text.Json.Serialization;
using MassTransit;
using PaymentsService.Consumers;
using PaymentsService.Contracts;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://0.0.0.0:8081");


Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/payments-service-.txt", rollingInterval: RollingInterval.Day)
    
    .Filter.ByExcluding(logEvent => 
        logEvent.Properties.TryGetValue("RequestPath", out var requestPath) && 
        requestPath.ToString().Contains("/healthz"))
    
    .Filter.ByExcluding(logEvent => 
        logEvent.MessageTemplate.Text.Contains("Executed DbCommand") ||
        logEvent.MessageTemplate.Text.Contains("SELECT") ||
        logEvent.MessageTemplate.Text.Contains("FROM"))
    .CreateLogger();

builder.Host.UseSerilog();


builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<PaymentsDbContext>(options =>
    options.UseNpgsql(connectionString));


builder.Services.AddScoped<IPaymentsService, PaymentsService.Services.PaymentsService>();


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


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Payments Service API",
        Version = "v1",
        Description = "Payments Service for E-commerce application"
    });
});

builder.Services.AddHealthChecks();

builder.Services.AddMassTransit(x =>
{
    x.UsingInMemory((context, cfg) =>
    {
        cfg.ConfigureEndpoints(context);
    });

    x.AddRider(rider =>
    {
        rider.AddConsumer<OrderCreatedConsumer>();

        rider.AddProducer<Guid, PaymentCompleted>("payment-completed-topic");
        rider.AddProducer<Guid, PaymentFailed>("payment-failed-topic");

        rider.UsingKafka((context, k) =>
        {
            var bootstrapServers = builder.Configuration["MessageBroker:BootstrapServers"];
            k.Host(bootstrapServers);
            
            k.TopicEndpoint<OrderCreated>("order-created-topic", "payments-service-consumer", e =>
            {
                e.CreateIfMissing();
                e.ConfigureConsumer<OrderCreatedConsumer>(context);
            });
        });
    });
});

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<PaymentsDbContext>();
    if (context.Database.GetPendingMigrations().Any())
    {
        context.Database.Migrate();
    }
}


Directory.CreateDirectory("logs");


if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Payments Service API v1"));
}

app.UseSerilogRequestLogging();
app.UseRouting();
app.UseCors();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/healthz");

app.Run();
