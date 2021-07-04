using System;
using System.Threading.Tasks;

namespace Lingua.Shared.Users
{
    public interface IUserService
    {
        Task<User> Create(User user);
        Task Update(User updated);
        Task Remove(Guid userId);
        Task<User> Get(Guid userId);
    }
}
