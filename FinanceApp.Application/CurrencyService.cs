using FinanceApp.Application.Interfacies;
using FinanceApp.Application.Models;
using FinanceApp.Domain;
using FinanceApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceApp.Application;

public class CurrencyService : ICurrencyService
{
    private readonly ICurrencyRepository _currencyRepo;
    private readonly IUserRepository _userRepo;

    public CurrencyService(ICurrencyRepository currencyRepo, IUserRepository userRepo)
    {
        _currencyRepo = currencyRepo;
        _userRepo = userRepo;
    }

    public async Task<List<Currency>> GetCurrenciesByUserAsync(GetCurrenciesByUserQuery query)
    {
        var user = await _userRepo.GetByNameAsync(query.UserName);
        if (user == null) throw new Exception("User not found.");

        var currencies = await _currencyRepo.GetAllAsync();
        return currencies
            .Where(c => user.Favorites.Contains(c.Id))
            .ToList();
    }
}

