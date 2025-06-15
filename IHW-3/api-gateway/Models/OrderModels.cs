using System.ComponentModel.DataAnnotations;

namespace ApiGateway.Models
{
    
    
    
    public class OrderModel
    {
        
        
        
        public Guid Id { get; set; }

        
        
        
        [Required]
        public Guid UserId { get; set; }

        
        
        
        public decimal TotalAmount { get; set; }

        
        
        
        public string Status { get; set; }

        
        
        
        public DateTime CreatedAt { get; set; }

        
        
        
        public List<OrderItemModel> Items { get; set; }
    }

    
    
    
    public class OrderItemModel
    {
        
        
        
        public Guid Id { get; set; }

        
        
        
        [Required]
        public Guid ProductId { get; set; }

        
        
        
        [Required]
        public int Quantity { get; set; }

        
        
        
        public decimal UnitPrice { get; set; }
    }

    
    
    
    public class CreateOrderModel
    {
        
        
        
        [Required]
        public Guid UserId { get; set; }

        
        
        
        [Required]
        public List<CreateOrderItemModel> Items { get; set; }
    }

    
    
    
    public class CreateOrderItemModel
    {
        
        
        
        [Required]
        public Guid ProductId { get; set; }

        
        
        
        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }
} 