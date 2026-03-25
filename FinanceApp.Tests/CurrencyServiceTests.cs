using FinanceApp.Application;
using FinanceApp.Application.Models;
using FinanceApp.Domain;
using FinanceApp.Domain.Entities;
using Moq;

namespace FinanceApp.Tests;

[TestFixture]
public class CurrencyServiceTests
{
    private Mock<ICurrencyRepository> _currencyRepoMock;
    private Mock<IUserRepository> _userRepoMock;
    private CurrencyService _currencyService;

    [SetUp]
    public void Setup()
    {
        _currencyRepoMock = new Mock<ICurrencyRepository>();
        _userRepoMock = new Mock<IUserRepository>();
        _currencyService = new CurrencyService(_currencyRepoMock.Object, _userRepoMock.Object);
    }

    [Test]
    public async Task GetCurrenciesByUserAsync_UserExists_ReturnsFavoriteCurrencies()
    {
        // Arrange
        var query = new GetCurrenciesByUserQuery("testuser");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "testuser",
            Password = "hashed-password",
            Favorites = ["USD", "AUD"]
        };

        var currencies = new List<Currency>
        {
            new() { Id = "USD", Name = "Доллар США", Rate = 80.9604m },
            new() { Id = "EUR", Name = "Евро", Rate = 93.9247m },
            new() { Id = "AUD", Name = "Австралийский доллар", Rate = 56.3970m }
        };

        _userRepoMock.Setup(repo => repo.GetByNameAsync(query.UserName))
            .ReturnsAsync(user);
        _currencyRepoMock.Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(currencies);

        // Act
        var result = await _currencyService.GetCurrenciesByUserAsync(query);

        // Assert
        Assert.IsNotNull(result);
        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result, Has.Some.Matches<Currency>(c =>
            c.Id == "USD" && c.Rate == 80.9604m));
        Assert.That(result, Has.Some.Matches<Currency>(c =>
            c.Id == "AUD" && c.Rate == 56.3970m));
    }

    [Test]
    public async Task GetCurrenciesByUserAsync_UserNotFound_ThrowsException()
    {
        // Arrange
        var query = new GetCurrenciesByUserQuery("nonexistent");

        _userRepoMock.Setup(repo => repo.GetByNameAsync(query.UserName))
            .ReturnsAsync((User)null);

        // Act & Assert
        var exception = Assert.ThrowsAsync<Exception>(
            async () => await _currencyService.GetCurrenciesByUserAsync(query));

        Assert.That(exception.Message, Is.EqualTo("User not found."));
    }

    [Test]
    public async Task GetCurrenciesByUserAsync_UserHasNoFavorites_ReturnsEmptyList()
    {
        // Arrange
        var query = new GetCurrenciesByUserQuery("userwithnofavorites");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "userwithnofavorites",
            Password = "hashed-password",
            Favorites = []
        };

        var currencies = new List<Currency>
        {
            new() { Id = "USD", Name = "Доллар США", Rate = 80.9604m },
            new() { Id = "EUR", Name = "Евро", Rate = 93.9247m },
            new() { Id = "AUD", Name = "Австралийский доллар", Rate = 56.3970m }
        };

        _userRepoMock.Setup(repo => repo.GetByNameAsync(query.UserName))
            .ReturnsAsync(user);
        _currencyRepoMock.Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(currencies);

        // Act
        var result = await _currencyService.GetCurrenciesByUserAsync(query);

        // Assert
        Assert.IsNotNull(result);
        Assert.That(result.Count, Is.EqualTo(0));
    }

    [Test]
    public async Task GetCurrenciesByUserAsync_NoMatchingCurrencies_ReturnsEmptyList()
    {
        // Arrange
        var query = new GetCurrenciesByUserQuery("testuser");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "testuser",
            Password = "hashed-password",
            Favorites = new List<string> { "JPY", "CAD" } // с некоторых пор курс валюты не устанавливается ЦБ
        };

        var currencies = new List<Currency>
        {
            new() { Id = "USD", Name = "Доллар США", Rate = 80.9604m },
            new() { Id = "EUR", Name = "Евро", Rate = 93.9247m },
            new() { Id = "AUD", Name = "Австралийский доллар", Rate = 56.3970m }
        };

        _userRepoMock.Setup(repo => repo.GetByNameAsync(query.UserName))
            .ReturnsAsync(user);
        _currencyRepoMock.Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(currencies);

        // Act
        var result = await _currencyService.GetCurrenciesByUserAsync(query);

        // Assert
        Assert.IsNotNull(result);
        Assert.That(result.Count, Is.EqualTo(0));
    }

    [Test]
    public async Task GetCurrenciesByUserAsync_EmptyCurrenciesList_ReturnsEmptyList()
    {
        // Arrange
        var query = new GetCurrenciesByUserQuery("testuser");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "testuser",
            Password = "hashed-password",
            Favorites = ["USD"]
        };

        var emptyCurrencies = new List<Currency>();

        _userRepoMock.Setup(repo => repo.GetByNameAsync(query.UserName))
            .ReturnsAsync(user);
        _currencyRepoMock.Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(emptyCurrencies);

        // Act
        var result = await _currencyService.GetCurrenciesByUserAsync(query);

        // Assert
        Assert.IsNotNull(result);
        Assert.That(result.Count, Is.EqualTo(0));
    }
}


