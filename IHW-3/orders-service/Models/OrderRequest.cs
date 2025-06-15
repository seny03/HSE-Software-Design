using System.ComponentModel.DataAnnotations;

namespace OrdersService.Models;

public class CreateOrderItemRequest
{
    [Required]
    public Guid ProductId { get; set; }
    
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
    public int Quantity { get; set; }
}

public class CreateOrderRequestDto
{
    [Required]
    public Guid UserId { get; set; }
    
    [Required]
    public List<CreateOrderItemRequest> Items { get; set; } = new();
} 