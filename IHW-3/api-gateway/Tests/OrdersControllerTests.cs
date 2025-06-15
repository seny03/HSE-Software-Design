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
    public class OrdersControllerTests
    {
        [Fact]
        public void GetOrders_ReturnsOkResult()
        {
            
            var controller = new OrdersController();
            var userId = Guid.NewGuid();

            
            var result = controller.GetOrders(userId);

            
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public void CreateOrder_WithValidModel_ReturnsAcceptedResult()
        {
            
            var controller = new OrdersController();
            var model = new CreateOrderModel
            {
                UserId = Guid.NewGuid(),
                Items = new List<CreateOrderItemModel>
                {
                    new CreateOrderItemModel
                    {
                        ProductId = Guid.NewGuid(),
                        Quantity = 1
                    }
                }
            };

            
            var result = controller.CreateOrder(model);

            
            Assert.IsType<AcceptedResult>(result);
        }

        [Fact]
        public void GetOrder_WithValidId_ReturnsOkResult()
        {
            
            var controller = new OrdersController();
            var orderId = Guid.NewGuid();

            
            var result = controller.GetOrder(orderId);

            
            Assert.IsType<OkResult>(result);
        }
    }
} 