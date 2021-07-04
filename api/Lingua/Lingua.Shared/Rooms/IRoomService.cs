using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lingua.Shared
{
    public interface IRoomService
    {
        Task<Room> Create(CreateRoomOptions options);
        Task Update(UpdateRoomOptions options);
        Task Remove(Guid roomId);
        Task<Room> Get(Guid roomId);
        Task<IEnumerable<Room>> Get(SearchRoomOptions options);
        Task AddParticipant(Guid roomId, User participant);
    }

}
