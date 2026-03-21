using Dapper;
using FinanceApp.Domain;
using FinanceApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinanceApp.Infrastructure;

public class CurrencyRepository : ICurrencyRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public CurrencyRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }

    public async Task AddOrUpdateAsync(Currency currency)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string checkQuery = """SELECT COUNT(1) FROM "Currencies" WHERE "Id" = @Id""";
        var exists = await connection.ExecuteScalarAsync<int>(checkQuery, new { currency.Id });

        if (exists == 0)
        {
            const string insertQuery = """INSERT INTO "Currencies" ("Id", "Rate") VALUES (@Id, @Rate)""";
            await connection.ExecuteAsync(insertQuery, currency);
        }
        else
        {
            const string updateQuery = """UPDATE "Currencies" SET "Rate" = @Rate WHERE "Id" = @Id""";
            await connection.ExecuteAsync(updateQuery, currency);
        }
    }

    public async Task<List<Currency>> GetAllAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        const string query = """SELECT * FROM "Currencies" """;
        return (await connection.QueryAsync<Currency>(query)).AsList();
    }

    public async Task<Currency?> GetByIdAsync(string id)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string query = """SELECT * FROM "Currencies" WHERE "Id" = @Id""";
        return await connection.QueryFirstOrDefaultAsync<Currency>(query, new { Id = id });
    }
}
