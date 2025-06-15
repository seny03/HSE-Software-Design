using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;
using ApiGateway.Models;

namespace ApiGateway.Controllers
{
    
    
    
    [ApiController]
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "v1")]
    public class ProductsController : ControllerBase
    {
        
        
        
        
        [HttpGet]
        [SwaggerOperation(
            Summary = "Получение списка всех продуктов",
            Description = "Возвращает список всех доступных продуктов",
            OperationId = "GetProducts",
            Tags = new[] { "Products" }
        )]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(List<ProductModel>), StatusCodes.Status200OK)]
        public IActionResult GetProducts()
        {
            
            return Ok();
        }

        
        
        
        
        
        [HttpGet("{id}")]
        [SwaggerOperation(
            Summary = "Получение продукта по ID",
            Description = "Возвращает информацию о продукте по указанному ID",
            OperationId = "GetProduct",
            Tags = new[] { "Products" }
        )]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ProductModel), StatusCodes.Status200OK)]
        public IActionResult GetProduct([FromRoute] Guid id)
        {
            
            return Ok();
        }
    }
} 