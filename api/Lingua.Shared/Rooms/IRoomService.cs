using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lingua.Shared
{
    public interface IRoomService
    {
        Task<List<Room>> Available(SearchRoomOptions options, Guid userId);
        Task<List<Room>> Upcoming(Guid userId);
        Task<List<Room>> Past(Guid userId);
        Task<List<Room>> Last();


        Task<Room> Create(CreateRoomOptions options, Guid userId);
        Task<Room> Enter(Guid roomId, Guid userId);
        Task<Room> Join(Guid roomId, Guid userId);
        Task<Room> Leave(Guid roomId, Guid userId);
        Task<Room> Remove(Guid roomId, Guid userId);
        Task<Room> Update(UpdateRoomOptions options, Guid userId);
    }
}