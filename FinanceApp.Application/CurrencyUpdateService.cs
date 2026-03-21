using FinanceApp.Domain;
using FinanceApp.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FinanceApp.Application;

public class CurrencyUpdateService(
    IHttpClientFactory httpClientFactory,
    IServiceScopeFactory scopeFactory) : BackgroundService
{
    const int UpdateIntervalMinutes = 30;
    const string XmlDailyUrl = "http://www.cbr.ru/scripts/XML_daily.asp";

    private ILogger<CurrencyUpdateService>? logger;
    private ICurrencyRepository? currencyRepo;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                logger = scope.ServiceProvider.GetRequiredService<ILogger<CurrencyUpdateService>>();
                currencyRepo = scope.ServiceProvider.GetRequiredService<ICurrencyRepository>();

                await UpdateCurrenciesAsync(stoppingToken);
            }

            await Task.Delay(TimeSpan.FromMinutes(UpdateIntervalMinutes), stoppingToken);
        }
    }

    private async Task UpdateCurrenciesAsync(CancellationToken ct)
    {
        try
        {
            var client = httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Accept", "application/xml");
            var xml = await client.GetStringAsync(XmlDailyUrl, ct);
            logger?.LogInformation(xml);

            var doc = XDocument.Parse(xml);
            foreach (var elem in doc.Descendants("Valute"))
            {
                var id = elem.Element("CharCode")?.Value;
                var name = elem.Element("Name")?.Value;
                var rateStr = elem.Element("VunitRate")?.Value?.Replace(',', '.');
                if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(rateStr)) continue;

                var rate = decimal.Parse(rateStr, NumberStyles.Float, CultureInfo.InvariantCulture);
                var currency = new Currency { Id = id, Name = name, Rate = rate };

                await currencyRepo!.AddOrUpdateAsync(currency);
            }
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Ошибка при загрузке/обновлении валют");
        }

    }
}

