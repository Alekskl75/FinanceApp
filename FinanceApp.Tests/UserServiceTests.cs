using FinanceApp.Application;
using FinanceApp.Application.Interfacies;
using FinanceApp.Application.Models;
using FinanceApp.Domain;
using FinanceApp.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace FinanceApp.Tests;

[TestFixture]
public class UserServiceTests
{
    private Mock<IUserRepository> _userRepoMock;
    private Mock<ICurrencyRepository> _currencyRepoMock;
    private Mock<IConfiguration> _configMock;
    private Mock<ILogger<UserService>> _loggerMock;
    private IUserService _userService;

    [SetUp]
    public void Setup()
    {
        _userRepoMock = new Mock<IUserRepository>();
        _currencyRepoMock = new Mock<ICurrencyRepository>();
        _configMock = new Mock<IConfiguration>();
        _loggerMock = new Mock<ILogger<UserService>>();
        _userService = new UserService(_userRepoMock.Object, _currencyRepoMock.Object, _configMock.Object, _loggerMock.Object);
    }

    [Test]
    public async Task RegisterAsync_ShouldReturnUserId_WhenUserDoesNotExist()
    {
        // Arrange
        var command = new RegisterUserCommand(Name: "testuser", Password: "password123");

        _userRepoMock
            .Setup(repo => repo.GetByNameAsync(command.Name))
            .ReturnsAsync((User)null);  // Пользователь не найден

        _userRepoMock
            .Setup(repo => repo.AddAsync(It.IsAny<User>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _userService.RegisterAsync(command);

        // Assert
        Assert.IsNotNull(result);
        Assert.That(result, Is.Not.EqualTo(Guid.Empty));
    }

    [Test]
    public async Task RegisterAsync_ShouldThrowException_WhenUserAlreadyExists()
    {
        // Arrange
        var command = new RegisterUserCommand(Name: "existinguser", Password: "password123");

        var existingUser = new User
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            Password = "hashedpassword"
        };

        _userRepoMock
            .Setup(repo => repo.GetByNameAsync(command.Name))
            .ReturnsAsync(existingUser);  // Пользователь уже существует

        // Act & Assert
        var exception = Assert.ThrowsAsync<Exception>(async () =>
            await _userService.RegisterAsync(command));

        Assert.That(exception.Message, Is.EqualTo("User already exists."));
    }

    [Test]
    public async Task RegisterAsync_ShouldHashPassword_BeforeSaving()
    {
        // Arrange
        var command = new RegisterUserCommand(Name: "newuser", Password: "plaintext");

        _userRepoMock
            .Setup(repo => repo.GetByNameAsync(command.Name))
            .ReturnsAsync((User)null);

        _userRepoMock
            .Setup(repo => repo.AddAsync(It.IsAny<User>()))
            .Callback<User>(user =>
            {
                // Проверяем, что пароль не хранится в открытом виде
                Assert.AreNotEqual("plaintext", user.Password);
                Assert.IsTrue(user.Password.Length > 0);  // Хэш не пустой
            })
            .Returns(Task.CompletedTask);

        // Act
        await _userService.RegisterAsync(command);
    }
}