using Microsoft.EntityFrameworkCore;
using OrdersService.Data;
using OrdersService.Services;
using MassTransit;
using OrdersService.Consumers;
using OrdersService.Hubs;
using Microsoft.AspNetCore.SignalR;
using OrdersService.Contracts;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Threading;
using Confluent.Kafka;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://0.0.0.0:8082");


Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/orders-service-.txt", rollingInterval: RollingInterval.Day)
    
    .Filter.ByExcluding(logEvent => 
        logEvent.Properties.TryGetValue("RequestPath", out var requestPath) && 
        requestPath.ToString().Contains("/healthz"))
    
    .Filter.ByExcluding(logEvent => 
        logEvent.MessageTemplate.Text.Contains("Executed DbCommand") ||
        logEvent.MessageTemplate.Text.Contains("SELECT") ||
        logEvent.MessageTemplate.Text.Contains("FROM"))
    .CreateLogger();

builder.Host.UseSerilog();


builder.Services.AddDbContext<OrdersDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddHttpClient();

builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddMassTransit(x =>
{
    x.AddEntityFrameworkOutbox<OrdersDbContext>(o =>
    {
        o.UsePostgres();
        o.UseBusOutbox();
    });

    x.UsingInMemory((context, cfg) =>
    {
        cfg.ConfigureEndpoints(context);
    });
    
    x.AddRider(rider =>
    {
        rider.AddConsumer<PaymentCompletedConsumer>();
        rider.AddConsumer<PaymentFailedConsumer>();
        
        rider.AddProducer<Guid, OrderCreated>("order-created-topic");

        rider.UsingKafka((context, k) =>
        {
            var bootstrapServers = builder.Configuration["MessageBroker:BootstrapServers"];
            k.Host(bootstrapServers);

            k.TopicEndpoint<PaymentCompleted>("payment-completed-topic", "orders-service-consumer-completed", e =>
            {
                e.ConfigureConsumer<PaymentCompletedConsumer>(context);
                e.CreateIfMissing();
            });
            
            k.TopicEndpoint<PaymentFailed>("payment-failed-topic", "orders-service-consumer-failed", e =>
            {
                e.ConfigureConsumer<PaymentFailedConsumer>(context);
                e.CreateIfMissing();
            });
        });
    });
});

builder.Services.AddHostedService<OutboxProcessor>();

builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddHealthChecks();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.WithOrigins("http://localhost:5001")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


WaitForKafkaTopics(builder.Configuration["MessageBroker:BootstrapServers"]);


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthorization();

app.UseSerilogRequestLogging();

app.MapControllers();
app.MapHub<OrderStatusHub>("/orderStatusHub");
app.MapHealthChecks("/healthz");


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<OrdersDbContext>();
    if (context.Database.GetPendingMigrations().Any())
    {
        context.Database.Migrate();
    }
}

app.Run();


void WaitForKafkaTopics(string bootstrapServers)
{
    var requiredTopics = new[] { "order-created-topic", "payment-completed-topic", "payment-failed-topic" };
    var config = new AdminClientConfig { BootstrapServers = bootstrapServers };
    var maxRetries = 30;
    var retryInterval = TimeSpan.FromSeconds(2);

    Console.WriteLine("Checking for Kafka topics...");

    for (int attempt = 1; attempt <= maxRetries; attempt++)
    {
        try
        {
            using (var adminClient = new AdminClientBuilder(config).Build())
            {
                var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(10));
                var availableTopics = metadata.Topics.Select(t => t.Topic).ToList();
                
                var missingTopics = requiredTopics.Except(availableTopics).ToList();
                
                if (!missingTopics.Any())
                {
                    Console.WriteLine("All required Kafka topics are available!");
                    return;
                }
                
                Console.WriteLine($"Attempt {attempt}/{maxRetries}: Waiting for topics: {string.Join(", ", missingTopics)}");
                Thread.Sleep(retryInterval);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error checking Kafka topics: {ex.Message}. Retrying in {retryInterval.TotalSeconds} seconds...");
            Thread.Sleep(retryInterval);
        }
    }
    
    Console.WriteLine("Proceeding anyway after maximum retries reached.");
}
