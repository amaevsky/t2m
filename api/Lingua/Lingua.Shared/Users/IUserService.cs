using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Lingua.Shared.Users
{
    public interface IUserService
    {
        Task<User> Create(User user);
        Task Update(User updated);
        Task Remove(Guid userId);
        Task<User> Get(Guid userId);
        Task<IEnumerable<User>> Get(Expression<Func<User, bool>> filter);
    }
}
