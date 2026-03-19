using FinanceApp.Infrastructure;
using FinanceApp.MigrationService;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection")));
builder.Services.AddScoped<MigrationService>();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();


// Получаем сервис миграций
//var migrationService = app.Services.GetRequiredService<MigrationService>();

//// 1. Создаём БД (если нет)
//migrationService.CreateDatabase();

//// 2. Применяем миграции
//migrationService.ApplyMigrations();

//// 3. Проверяем статус
//var (applied, available) = migrationService.GetMigrationStatus();
//Console.WriteLine($"Applied: {applied.Length}, Available: {available.Length}");


app.Run();
