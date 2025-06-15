using System.ComponentModel.DataAnnotations;

namespace ApiGateway.Models
{
    
    
    
    public class PaymentAccountModel
    {
        
        
        
        public Guid Id { get; set; }

        
        
        
        [Required]
        public Guid UserId { get; set; }

        
        
        
        public decimal Balance { get; set; }

        
        
        
        public DateTime CreatedAt { get; set; }
    }

    
    
    
    public class CreatePaymentAccountModel
    {
        
        
        
        [Required]
        public Guid UserId { get; set; }

        
        
        
        public decimal InitialBalance { get; set; } = 0;
    }

    
    
    
    public class BalanceOperationModel
    {
        
        
        
        [Required]
        public Guid UserId { get; set; }

        
        
        
        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }
    }

    
    
    
    public class BalanceResponseModel
    {
        
        
        
        public Guid UserId { get; set; }

        
        
        
        public decimal Balance { get; set; }
    }
} 