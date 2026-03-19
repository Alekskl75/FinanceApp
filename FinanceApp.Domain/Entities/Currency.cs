namespace FinanceApp.Domain.Entities;

public class Currency
{
    public string Id { get; set; }  // 3 uppercase chars
    public string Name { get; set; }
    public decimal Rate { get; set; }
}

