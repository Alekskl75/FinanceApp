using FinanceApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceApp.Domain;

public interface ICurrencyRepository
{
    Task<Currency?> GetByIdAsync(string id);
    Task<List<Currency>> GetAllAsync();
    Task AddOrUpdateAsync(Currency currency);
}
