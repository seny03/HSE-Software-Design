using Microsoft.AspNetCore.Mvc;
using ApiGateway.Models;
using System.Collections.Generic;

namespace ApiGateway.Controllers;

[ApiController]
[Route("api")]
public class ApiDocsController : ControllerBase
{
    
    
    
    
    
    [HttpGet("orders")]
    [ProducesResponseType(typeof(List<Order>), StatusCodes.Status200OK)]
    public IActionResult GetOrders([FromQuery] Guid userId)
    {
        
        
        return Ok();
    }
    
    
    
    
    
    [HttpPost("orders")]
    [ProducesResponseType(typeof(Order), StatusCodes.Status201Created)]
    public IActionResult CreateOrder([FromBody] CreateOrderRequest request)
    {
        
        
        return Ok();
    }
    
    
    
    
    
    [HttpGet("products")]
    [ProducesResponseType(typeof(List<Product>), StatusCodes.Status200OK)]
    public IActionResult GetProducts()
    {
        
        
        return Ok();
    }
    
    
    
    
    
    
    [HttpGet("products/{id}")]
    [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetProduct(Guid id)
    {
        
        
        return Ok();
    }
    
    
    
    
    
    
    [HttpGet("payments")]
    [ProducesResponseType(typeof(List<Payment>), StatusCodes.Status200OK)]
    public IActionResult GetPayments([FromQuery] Guid userId)
    {
        
        
        return Ok();
    }
    
    
    
    
    
    [HttpPost("payments")]
    [ProducesResponseType(typeof(Payment), StatusCodes.Status201Created)]
    public IActionResult CreatePayment([FromBody] CreatePaymentRequest request)
    {
        
        
        return Ok();
    }
    
    public class CreateOrderRequest
    {
        public Guid UserId { get; set; }
        public List<OrderItemRequest> Items { get; set; } = new();
    }
    
    public class OrderItemRequest
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
    
    public class CreatePaymentRequest
    {
        public Guid OrderId { get; set; }
        public decimal Amount { get; set; }
    }
} 