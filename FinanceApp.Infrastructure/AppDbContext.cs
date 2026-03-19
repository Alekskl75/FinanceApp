namespace FinanceApp.Infrastructure;

using FinanceApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Currency> Currencies { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Currency>(b =>
        {
            b.HasKey(c => c.Id);
            b.Property(c => c.Id).HasMaxLength(3).IsRequired();
            b.Property(c => c.Name).IsRequired();
            b.Property(c => c.Rate).HasColumnType("decimal(18,6)").IsRequired();
        });

        builder.Entity<User>(b =>
        {
            b.HasKey(u => u.Id);
            b.Property(u => u.Name).IsRequired();
            b.Property(u => u.Password).IsRequired();
            b.HasData(new User { Id = Guid.NewGuid(), Name = "admin", Password = "admin" });
        });
    }
}