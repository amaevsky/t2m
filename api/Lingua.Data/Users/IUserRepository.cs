using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Lingua.Data
{
    public interface IUserRepository
    {
        Task<User> Create(User user);
        Task Update(User updated);
        Task Remove(Guid userId);
        Task<User> Get(Guid userId);
        Task<IEnumerable<User>> Get(Expression<Func<User, bool>> filter);
    }
}
