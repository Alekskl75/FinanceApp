using FinanceApp.Application.Models;
using FinanceApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceApp.Application.Interfacies;

public interface ICurrencyService
{
    Task<List<Currency>> GetCurrenciesByUserAsync(GetCurrenciesByUserQuery query);
}
