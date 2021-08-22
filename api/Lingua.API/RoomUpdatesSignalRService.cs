using AutoMapper;
using Lingua.API.Realtime;
using Lingua.API.ViewModels;
using Lingua.Shared;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Lingua.API
{
    public class RoomUpdatesSignalRService : IRoomUpdatesObserver
    {
        private readonly IHubContext<RoomsHub, IRoomsRealtimeClient> _roomsHub;
        private readonly IMapper _mapper;

        public RoomUpdatesSignalRService(IHubContext<RoomsHub, IRoomsRealtimeClient> roomsHub, IMapper mapper)
        {
            _roomsHub = roomsHub;
            _mapper = mapper;
        }

        public Task OnUpdate(RoomUpdateType updateType, Room room, User user)
        {
            var vm = _mapper.Map<RoomViewModel>(room);

            switch (updateType)
            {
                case RoomUpdateType.Created:
                    return _roomsHub.Clients.All.OnAdd(vm, user.Id);
                case RoomUpdateType.Updated:
                    return _roomsHub.Clients.All.OnUpdate(vm, user.Id);
                case RoomUpdateType.Entered:
                    return _roomsHub.Clients.All.OnEnter(vm, user.Id);
                case RoomUpdateType.Left:
                    return _roomsHub.Clients.All.OnLeave(vm, user.Id);
                case RoomUpdateType.Removed:
                    return _roomsHub.Clients.All.OnRemove(vm, user.Id);
                case RoomUpdateType.Joined: break;
            }

            return Task.CompletedTask;
        }
    }
}
