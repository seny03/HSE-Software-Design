using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using ApiGateway.Controllers;
using ApiGateway.Models;
using System.Collections.Generic;

namespace ApiGateway.Tests
{
    public class ProductsControllerTests
    {
        [Fact]
        public void GetProducts_ReturnsOkResult()
        {
            
            var controller = new ProductsController();

            
            var result = controller.GetProducts();

            
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public void GetProduct_WithValidId_ReturnsOkResult()
        {
            
            var controller = new ProductsController();
            var validId = Guid.NewGuid();

            
            var result = controller.GetProduct(validId);

            
            Assert.IsType<OkResult>(result);
        }
    }
} 