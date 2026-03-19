using FinanceApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceApp.Domain;

public interface IUserRepository
{
    Task<User> GetByNameAsync(string name);
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task<User> GetByIdAsync(Guid userId);
}
