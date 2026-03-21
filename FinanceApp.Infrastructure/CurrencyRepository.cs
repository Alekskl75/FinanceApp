using FinanceApp.Domain;
using FinanceApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceApp.Infrastructure;

public class CurrencyRepository(AppDbContext context) : ICurrencyRepository
{
    public async Task AddOrUpdateAsync(Currency currency)
    {
        var entity = await context.Currencies.FirstOrDefaultAsync(x => x.Id == currency.Id);
        if (entity == null)
            await context.Currencies.AddAsync(currency);
        else
            entity.Rate = currency.Rate;
        await context.SaveChangesAsync();
    }

    public async Task<List<Currency>> GetAllAsync()
    {
        return await context.Currencies.AsNoTracking().ToListAsync();
    }

    public async Task<Currency?> GetByIdAsync(string id)
    {
        return await context.Currencies.FirstOrDefaultAsync(x => x.Id == id);
    }
}
