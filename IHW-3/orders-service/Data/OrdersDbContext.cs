using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;
using OrdersService.Models;

namespace OrdersService.Data;

public class OrdersDbContext : DbContext
{
    public OrdersDbContext(DbContextOptions<OrdersDbContext> options) : base(options)
    {
    }

    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<OrdersService.Models.OutboxMessage> OutboxMessages { get; set; }
    
    
    public DbSet<MassTransit.EntityFrameworkCoreIntegration.InboxState> InboxStates { get; set; }
    public DbSet<MassTransit.EntityFrameworkCoreIntegration.OutboxState> OutboxStates { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Order>()
            .HasMany(o => o.Items)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);
            
        
        modelBuilder.Entity<MassTransit.EntityFrameworkCoreIntegration.InboxState>(b =>
        {
            b.ToTable("InboxStates");
            b.HasKey(i => i.Id);
        });

        modelBuilder.Entity<MassTransit.EntityFrameworkCoreIntegration.OutboxState>(b =>
        {
            b.ToTable("OutboxStates");
            b.HasKey(o => o.OutboxId);
        });

        modelBuilder.Entity<MassTransit.EntityFrameworkCoreIntegration.OutboxMessage>(b =>
        {
            b.ToTable("OutboxMessagesEF");
            b.HasKey(o => o.SequenceNumber);
        });

        
        modelBuilder.Entity<Product>().HasData(
            new Product { Id = Guid.NewGuid(), Name = "Laptop", Price = 1200.00m },
            new Product { Id = Guid.NewGuid(), Name = "Smartphone", Price = 800.00m },
            new Product { Id = Guid.NewGuid(), Name = "Headphones", Price = 150.00m },
            new Product { Id = Guid.NewGuid(), Name = "Monitor", Price = 300.00m },
            new Product { Id = Guid.NewGuid(), Name = "Keyboard", Price = 80.00m }
        );
    }
} 