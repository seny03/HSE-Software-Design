using Microsoft.AspNetCore.Mvc;
using PaymentsService.Contracts;
using PaymentsService.Models;
using PaymentsService.Services;

namespace PaymentsService.Controllers;

[ApiController]
[Route("[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentsService _paymentsService;

    public PaymentsController(IPaymentsService paymentsService)
    {
        _paymentsService = paymentsService;
    }

    [HttpPost("account")]
    public async Task<IActionResult> CreateAccount([FromBody] CreateAccountRequest request)
    {
        var account = await _paymentsService.CreateAccountAsync(request.UserName);
        return Ok(account);
    }

    [HttpPost("account/deposit")]
    public async Task<IActionResult> Deposit(Guid userId, decimal amount)
    {
        var account = await _paymentsService.DepositAsync(userId, amount);
        if (account == null)
        {
            return NotFound("Account not found");
        }
        return Ok(account);
    }

    [HttpPost("account/withdraw")]
    public async Task<IActionResult> Withdraw(Guid userId, decimal amount)
    {
        var (success, message) = await _paymentsService.WithdrawAsync(userId, amount);
        if (!success)
        {
            if (message == "Account not found")
            {
                return NotFound(message);
            }
            return BadRequest(message);
        }
        return Ok(new { message });
    }

    [HttpGet("account/balance")]
    public async Task<IActionResult> GetBalance(Guid userId)
    {
        var balance = await _paymentsService.GetBalanceAsync(userId);
        if (balance == null)
        {
            return NotFound("Account not found");
        }
        return Ok(balance);
    }

    [HttpGet("accounts")]
    public async Task<IActionResult> GetAllAccounts()
    {
        var accounts = await _paymentsService.GetAllAccountsAsync();
        return Ok(accounts);
    }
} 