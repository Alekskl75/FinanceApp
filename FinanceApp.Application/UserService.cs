using FinanceApp.Application.Interfacies;
using FinanceApp.Application.Models;
using FinanceApp.Domain;
using FinanceApp.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace FinanceApp.Application;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepo;
    private readonly IConfiguration _config;

    public UserService(IUserRepository userRepo, IConfiguration config)
    {
        _userRepo = userRepo;
        _config = config;
    }

    public async Task<Guid> RegisterAsync(RegisterUserCommand command)
    {
        if (await _userRepo.GetByNameAsync(command.Name) != null)
            throw new Exception("User already exists.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            Password = ComputeSha256Hash(command.Password)
        };

        await _userRepo.AddAsync(user);
        return user.Id;
    }

    public async Task<string> LoginAsync(LoginUserQuery query)
    {
        var user = await _userRepo.GetByNameAsync(query.Name);
        if (user == null || !VerifyPassword(query.Password, user.Password))
            throw new Exception("Invalid credentials.");

        return GenerateJwtToken(user);
    }

    public async Task LogoutAsync(Guid userId)
    {
        // В простой реализации — ничего не делаем (JWT живёт на стороне клиента)
        // Для строгой logout можно использовать чёрный список токенов
    }

    public async Task UpdateFavoritesAsync(UpdateFavoritesCommand command)
    {
        var user = await _userRepo.GetByIdAsync(command.UserId);
        if (user == null) throw new Exception("User not found.");

        user.Favorites = command.Favorites;
        await _userRepo.UpdateAsync(user);
    }

    public static string HashPassword(string password)
    {
        return ComputeSha256Hash(password);
    }

    private static string ComputeSha256Hash(string input)
    {
        using var sha256 = SHA256.Create();
        byte[] inputBytes = Encoding.UTF8.GetBytes(input);

        byte[] hashBytes = sha256.ComputeHash(inputBytes);

        StringBuilder sb = new StringBuilder();
        foreach (byte b in hashBytes)
        {
            sb.Append(b.ToString("x2"));
        }
        return sb.ToString();
    }

    private bool VerifyPassword(string input, string storedHash)
    {
        // Проверить хешированный пароль
        return input == storedHash; // Заменить на реальную проверку
    }

    private string GenerateJwtToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Name)
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

