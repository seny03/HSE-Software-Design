using System.ComponentModel.DataAnnotations;

namespace ApiGateway.Models
{
    
    
    
    public class ProductModel
    {
        
        
        
        public Guid Id { get; set; }

        
        
        
        [Required]
        public string Name { get; set; }

        
        
        
        public string Description { get; set; }

        
        
        
        [Required]
        public decimal Price { get; set; }

        
        
        
        public int Stock { get; set; }
    }
} 