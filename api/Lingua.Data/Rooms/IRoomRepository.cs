using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Lingua.Data
{
    public interface IRoomRepository
    {
        Task<Room> Create(Room room);
        Task Update(Room updated);
        Task Remove(Guid roomId);
        Task<Room> Get(Guid roomId);
        Task<IEnumerable<Room>> Get(Expression<Func<Room, bool>> filter = null);
    }

}
