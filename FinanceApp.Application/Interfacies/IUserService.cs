using FinanceApp.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceApp.Application.Interfacies;

public interface IUserService
{
    Task<Guid> RegisterAsync(RegisterUserCommand command);
    Task<string> LoginAsync(LoginUserQuery query);
    Task LogoutAsync(Guid userId);
    Task UpdateFavoritesAsync(UpdateFavoritesCommand command);
}
