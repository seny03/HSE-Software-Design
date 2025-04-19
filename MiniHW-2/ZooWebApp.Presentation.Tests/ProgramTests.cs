using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using System.Reflection;
using Xunit;
using ZooWebApp.Application.Services;
using ZooWebApp.Domain.Common.Interfaces;
using ZooWebApp.Infrastructure.Repositories;

namespace ZooWebApp.Presentation.Tests;

public class ProgramTests
{
    [Fact]
    public void CreateHostBuilder_ConfiguresWebHost()
    {
        // Arrange & Act
        var builder = WebApplication.CreateBuilder(new string[] { });
        
        // Use reflection to access Program.ConfigureServices since it's internal
        var programType = typeof(Program);
        var configureServicesMethod = programType.GetMethod("ConfigureServices", 
            BindingFlags.NonPublic | BindingFlags.Static);
        
        // Act
        configureServicesMethod?.Invoke(null, new object[] { builder.Services });
        
        // Assert
        // Verify that required services are registered
        var serviceProvider = builder.Services.BuildServiceProvider();
        Assert.NotNull(serviceProvider.GetService<IAnimalRepository>());
        Assert.NotNull(serviceProvider.GetService<IEnclosureRepository>());
        Assert.NotNull(serviceProvider.GetService<IAnimalService>());
        Assert.NotNull(serviceProvider.GetService<IZooStatisticsService>());
    }
    
    [Fact]
    public void ConfigureServices_RegistersRepositories()
    {
        // Arrange
        var services = new ServiceCollection();
        
        // Use reflection to access Program.ConfigureServices since it's internal
        var programType = typeof(Program);
        var configureServicesMethod = programType.GetMethod("ConfigureServices", 
            BindingFlags.NonPublic | BindingFlags.Static);
        
        // Act
        configureServicesMethod?.Invoke(null, new object[] { services });
        
        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var animalRepository = serviceProvider.GetService<IAnimalRepository>();
        var enclosureRepository = serviceProvider.GetService<IEnclosureRepository>();
        
        Assert.NotNull(animalRepository);
        Assert.NotNull(enclosureRepository);
        Assert.IsType<InMemoryAnimalRepository>(animalRepository);
        Assert.IsType<InMemoryEnclosureRepository>(enclosureRepository);
    }
    
    [Fact]
    public void ConfigureServices_RegistersApplicationServices()
    {
        // Arrange
        var services = new ServiceCollection();
        
        // Use reflection to access Program.ConfigureServices since it's internal
        var programType = typeof(Program);
        var configureServicesMethod = programType.GetMethod("ConfigureServices", 
            BindingFlags.NonPublic | BindingFlags.Static);
        
        // Act
        configureServicesMethod?.Invoke(null, new object[] { services });
        
        // Assert
        var serviceProvider = services.BuildServiceProvider();
        Assert.NotNull(serviceProvider.GetService<IAnimalService>());
        Assert.NotNull(serviceProvider.GetService<IZooStatisticsService>());
    }
}
