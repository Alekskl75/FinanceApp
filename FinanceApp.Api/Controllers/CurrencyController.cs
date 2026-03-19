using FinanceApp.Application.Interfacies;
using FinanceApp.Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FinanceApp.Api.Controllers;

[ApiController]
[Route("api/currencies")]
public class CurrencyController : ControllerBase
{
    private readonly ICurrencyService _currencyService;

    public CurrencyController(ICurrencyService currencyService)
    {
        _currencyService = currencyService;
    }

    [HttpGet("user/{userId}")]
    [Authorize]
    public async Task<IActionResult> GetCurrenciesByUser(Guid userId)
    {
        var query = new GetCurrenciesByUserQuery(userId);
        var currencies = await _currencyService.GetCurrenciesByUserAsync(query);
        return Ok(currencies);
    }
}

