using System.Reflection;
using Microsoft.Extensions.Logging;
using System.Text;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Net.Http.Json;

var builder = WebApplication.CreateBuilder(args);


builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Information);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.WithOrigins("http://localhost:5001")
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});


builder.Services.AddControllers();


builder.Services.AddHttpClient();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "API Gateway",
        Version = "v1",
        Description = "API Gateway for E-commerce Microservices"
    });
    
    
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
    
    
    c.AddServer(new Microsoft.OpenApi.Models.OpenApiServer
    {
        Url = "/",
        Description = "API Gateway"
    });
    
    
    c.DocInclusionPredicate((docName, apiDesc) => true);
    
    
    c.EnableAnnotations();
    c.TagActionsBy(api => new[] { api.GroupName ?? api.HttpMethod });
});


builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Gateway v1");
        c.DisplayRequestDuration();
        c.EnableDeepLinking();
        c.EnableFilter();
        c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
    });
}

app.UseCors("AllowAll");


app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    
    
    logger.LogInformation("Request: {0} {1}", 
        context.Request.Method, 
        context.Request.Path);
    
    
    var originalBodyStream = context.Response.Body;
    
    try
    {
        
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;
        
        
        await next.Invoke();
        
        
        responseBody.Seek(0, SeekOrigin.Begin);
        var responseText = await new StreamReader(responseBody).ReadToEndAsync();
        
        
        logger.LogInformation("Response: {0} for {1} {2}, Content-Length: {3}, Content: {4}", 
            context.Response.StatusCode,
            context.Request.Method, 
            context.Request.Path,
            responseText.Length,
            responseText.Length > 100 ? responseText.Substring(0, 100) + "..." : responseText);
        
        
        responseBody.Seek(0, SeekOrigin.Begin);
        await responseBody.CopyToAsync(originalBodyStream);
    }
    finally
    {
        
        context.Response.Body = originalBodyStream;
    }
});


app.MapControllers();


app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    var httpClientFactory = context.RequestServices.GetRequiredService<IHttpClientFactory>();
    
    
    
    
    if (context.Request.Method == "GET" && context.Request.Path.Value == "/api/products")
    {
        logger.LogInformation("Handling GET /api/products request");
        
        try
        {
            var client = httpClientFactory.CreateClient();
            var response = await client.GetAsync("http://orders-service:8082/products");
            
            logger.LogInformation("Response status code: {0}", response.StatusCode);
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                logger.LogInformation("Response content: {0}", content);
                
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(content);
                return;
            }
            else
            {
                context.Response.StatusCode = (int)response.StatusCode;
                await context.Response.WriteAsync($"Error from orders service: {response.StatusCode}");
                return;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error handling GET /api/products");
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync($"Error: {ex.Message}");
            return;
        }
    }
    
    
    else if (context.Request.Method == "GET" && context.Request.Path.Value?.StartsWith("/api/products/") == true)
    {
        var productId = context.Request.Path.Value.Split('/').Last();
        logger.LogInformation("Handling GET /api/products/{0} request", productId);
        
        try
        {
            var client = httpClientFactory.CreateClient();
            var response = await client.GetAsync($"http://orders-service:8082/products/{productId}");
            
            logger.LogInformation("Response status code: {0}", response.StatusCode);
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                logger.LogInformation("Response content: {0}", content);
                
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(content);
                return;
            }
            else
            {
                context.Response.StatusCode = (int)response.StatusCode;
                await context.Response.WriteAsync($"Error from orders service: {response.StatusCode}");
                return;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error handling GET /api/products/{0}", productId);
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync($"Error: {ex.Message}");
            return;
        }
    }
    
    
    else if (context.Request.Method == "GET" && context.Request.Path.Value == "/api/orders")
    {
        var userId = context.Request.Query["userId"].ToString();
        logger.LogInformation("Handling GET /api/orders request with userId: {0}", userId);
        
        try
        {
            var client = httpClientFactory.CreateClient();
            var url = string.IsNullOrEmpty(userId) 
                ? "http://orders-service:8082/orders" 
                : $"http://orders-service:8082/orders?userId={userId}";
                
            var response = await client.GetAsync(url);
            
            logger.LogInformation("Response status code: {0}", response.StatusCode);
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                logger.LogInformation("Response content: {0}", content);
                
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(content);
                return;
            }
            else
            {
                context.Response.StatusCode = (int)response.StatusCode;
                await context.Response.WriteAsync($"Error from orders service: {response.StatusCode}");
                return;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error handling GET /api/orders");
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync($"Error: {ex.Message}");
            return;
        }
    }
    
    
    else if (context.Request.Method == "POST" && context.Request.Path.Value == "/api/orders")
    {
        logger.LogInformation("Handling POST /api/orders request");
        
        try
        {
            var client = httpClientFactory.CreateClient();
            
            
            using var reader = new StreamReader(context.Request.Body);
            var requestBody = await reader.ReadToEndAsync();
            
            
            var content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("http://orders-service:8082/orders", content);
            
            logger.LogInformation("Response status code: {0}", response.StatusCode);
            
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                logger.LogInformation("Response content: {0}", responseContent);
                
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)response.StatusCode;
                await context.Response.WriteAsync(responseContent);
                return;
            }
            else
            {
                context.Response.StatusCode = (int)response.StatusCode;
                await context.Response.WriteAsync($"Error from orders service: {response.StatusCode}");
                return;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error handling POST /api/orders");
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync($"Error: {ex.Message}");
            return;
        }
    }
    
    
    else if (context.Request.Method == "GET" && context.Request.Path.Value?.StartsWith("/api/orders/") == true && !context.Request.Path.Value.Contains("user"))
    {
        var orderId = context.Request.Path.Value.Split('/').Last();
        logger.LogInformation("Handling GET /api/orders/{0} request", orderId);
        
        try
        {
            var client = httpClientFactory.CreateClient();
            var response = await client.GetAsync($"http://orders-service:8082/orders/{orderId}");
            
            logger.LogInformation("Response status code: {0}", response.StatusCode);
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                logger.LogInformation("Response content: {0}", content);
                
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(content);
                return;
            }
            else
            {
                context.Response.StatusCode = (int)response.StatusCode;
                await context.Response.WriteAsync($"Error from orders service: {response.StatusCode}");
                return;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error handling GET /api/orders/{0}", orderId);
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync($"Error: {ex.Message}");
            return;
        }
    }
    
    
    else if (context.Request.Method == "GET" && context.Request.Path.Value?.StartsWith("/api/orders/user/") == true)
    {
        var userId = context.Request.Path.Value.Split('/').Last();
        logger.LogInformation("Handling GET /api/orders/user/{0} request", userId);
        
        try
        {
            var client = httpClientFactory.CreateClient();
            var response = await client.GetAsync($"http://orders-service:8082/orders/user/{userId}");
            
            logger.LogInformation("Response status code: {0}", response.StatusCode);
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                logger.LogInformation("Response content: {0}", content);
                
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(content);
                return;
            }
            else
            {
                context.Response.StatusCode = (int)response.StatusCode;
                await context.Response.WriteAsync($"Error from orders service: {response.StatusCode}");
                return;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error handling GET /api/orders/user/{0}", userId);
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync($"Error: {ex.Message}");
            return;
        }
    }
    
    
    
    
    else if (context.Request.Method == "POST" && context.Request.Path.Value == "/api/payments/account")
    {
        logger.LogInformation("Handling POST /api/payments/account request");
        
        try
        {
            var client = httpClientFactory.CreateClient();
            
            
            using var reader = new StreamReader(context.Request.Body);
            var requestBody = await reader.ReadToEndAsync();
            
            
            var content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("http://payments-service:8081/payments/account", content);
            
            logger.LogInformation("Response status code: {0}", response.StatusCode);
            
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                logger.LogInformation("Response content: {0}", responseContent);
                
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)response.StatusCode;
                await context.Response.WriteAsync(responseContent);
                return;
            }
            else
            {
                context.Response.StatusCode = (int)response.StatusCode;
                await context.Response.WriteAsync($"Error from payments service: {response.StatusCode}");
                return;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error handling POST /api/payments/account");
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync($"Error: {ex.Message}");
            return;
        }
    }
    
    
    else if (context.Request.Method == "POST" && context.Request.Path.Value == "/api/payments/account/deposit")
    {
        var userId = context.Request.Query["userId"].ToString();
        var amount = context.Request.Query["amount"].ToString();
        
        logger.LogInformation("Handling POST /api/payments/account/deposit request with userId: {0}, amount: {1}", userId, amount);
        
        try
        {
            var client = httpClientFactory.CreateClient();
            var response = await client.PostAsync($"http://payments-service:8081/payments/account/deposit?userId={userId}&amount={amount}", null);
            
            logger.LogInformation("Response status code: {0}", response.StatusCode);
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                logger.LogInformation("Response content: {0}", content);
                
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(content);
                return;
            }
            else
            {
                context.Response.StatusCode = (int)response.StatusCode;
                await context.Response.WriteAsync($"Error from payments service: {response.StatusCode}");
                return;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error handling POST /api/payments/account/deposit");
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync($"Error: {ex.Message}");
            return;
        }
    }
    
    
    else if (context.Request.Method == "POST" && context.Request.Path.Value == "/api/payments/account/withdraw")
    {
        var userId = context.Request.Query["userId"].ToString();
        var amount = context.Request.Query["amount"].ToString();
        
        logger.LogInformation("Handling POST /api/payments/account/withdraw request with userId: {0}, amount: {1}", userId, amount);
        
        try
        {
            var client = httpClientFactory.CreateClient();
            var response = await client.PostAsync($"http://payments-service:8081/payments/account/withdraw?userId={userId}&amount={amount}", null);
            
            logger.LogInformation("Response status code: {0}", response.StatusCode);
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                logger.LogInformation("Response content: {0}", content);
                
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(content);
                return;
            }
            else
            {
                context.Response.StatusCode = (int)response.StatusCode;
                await context.Response.WriteAsync($"Error from payments service: {response.StatusCode}");
                return;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error handling POST /api/payments/account/withdraw");
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync($"Error: {ex.Message}");
            return;
        }
    }
    
    
    else if (context.Request.Method == "GET" && context.Request.Path.Value == "/api/payments/account/balance")
    {
        var userId = context.Request.Query["userId"].ToString();
        logger.LogInformation("Handling GET /api/payments/account/balance request with userId: {0}", userId);
        
        try
        {
            var client = httpClientFactory.CreateClient();
            var response = await client.GetAsync($"http://payments-service:8081/payments/account/balance?userId={userId}");
            
            logger.LogInformation("Response status code: {0}", response.StatusCode);
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                logger.LogInformation("Response content: {0}", content);
                
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(content);
                return;
            }
            else
            {
                context.Response.StatusCode = (int)response.StatusCode;
                await context.Response.WriteAsync($"Error from payments service: {response.StatusCode}");
                return;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error handling GET /api/payments/account/balance");
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync($"Error: {ex.Message}");
            return;
        }
    }
    
    
    else if (context.Request.Method == "GET" && context.Request.Path.Value == "/api/payments/accounts")
    {
        logger.LogInformation("Handling GET /api/payments/accounts request");
        
        try
        {
            var client = httpClientFactory.CreateClient();
            var response = await client.GetAsync("http://payments-service:8081/payments/accounts");
            
            logger.LogInformation("Response status code: {0}", response.StatusCode);
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                logger.LogInformation("Response content: {0}", content);
                
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(content);
                return;
            }
            else
            {
                context.Response.StatusCode = (int)response.StatusCode;
                await context.Response.WriteAsync($"Error from payments service: {response.StatusCode}");
                return;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error handling GET /api/payments/accounts");
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync($"Error: {ex.Message}");
            return;
        }
    }
    
    await next();
});

app.MapReverseProxy();

app.Run();
