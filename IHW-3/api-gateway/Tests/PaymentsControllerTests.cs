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
    public class PaymentsControllerTests
    {
        [Fact]
        public void CreateAccount_WithValidModel_ReturnsCreatedResult()
        {
            
            var controller = new PaymentsController();
            var model = new CreatePaymentAccountModel
            {
                UserId = Guid.NewGuid()
            };

            
            var result = controller.CreateAccount(model);

            
            Assert.IsType<CreatedResult>(result);
        }

        [Fact]
        public void DepositToAccount_WithValidParameters_ReturnsOkResult()
        {
            
            var controller = new PaymentsController();
            var userId = Guid.NewGuid();
            decimal amount = 100;

            
            var result = controller.DepositToAccount(userId, amount);

            
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public void WithdrawFromAccount_WithValidParameters_ReturnsOkResult()
        {
            
            var controller = new PaymentsController();
            var userId = Guid.NewGuid();
            decimal amount = 50;

            
            var result = controller.WithdrawFromAccount(userId, amount);

            
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public void GetAccountBalance_WithValidUserId_ReturnsOkResult()
        {
            
            var controller = new PaymentsController();
            var userId = Guid.NewGuid();

            
            var result = controller.GetAccountBalance(userId);

            
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public void GetAllAccounts_ReturnsOkResult()
        {
            
            var controller = new PaymentsController();

            
            var result = controller.GetAllAccounts();

            
            Assert.IsType<OkResult>(result);
        }
    }
} 