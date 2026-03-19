namespace FinanceApp.MigrationService;

using FinanceApp.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;

public class MigrationService(AppDbContext context, ILogger<MigrationService> logger)
{

    /// <summary>
    /// Применяет все pending-миграции к БД
    /// </summary>
    public void ApplyMigrations()
    {
        var applied = context.Database.GetAppliedMigrations();
        var pending = context.Database.GetPendingMigrations();

        Console.WriteLine($"Applied migrations: {string.Join(", ", applied)}");
        Console.WriteLine($"Pending migrations: {string.Join(", ", pending)}");

        if (pending.Any())
        {
            Console.WriteLine("Applying migrations...");
            context.Database.Migrate();
            Console.WriteLine("Migrations applied successfully.");
        }
        else
        {
            Console.WriteLine("No pending migrations.");
        }
    }

    /// <summary>
    /// Проверяет, существует ли БД
    /// </summary>
    public bool DatabaseExists()
    {
        return context.Database.CanConnect();
    }

    /// <summary>
    /// Получает список всех миграций (применённых и доступных)
    /// </summary>
    public (string[] applied, string[] available) GetMigrationStatus()
    {
        var applied = context.Database.GetAppliedMigrations().ToArray();
        var available = context.Database.GetMigrations().ToArray();
        return (applied, available);
    }
}

