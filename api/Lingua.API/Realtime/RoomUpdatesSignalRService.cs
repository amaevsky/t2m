using AutoMapper;
using Lingua.API.Realtime;
using Lingua.API.ViewModels;
using Lingua.Services.Rooms.Events;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using System.Threading;
using System.Threading.Tasks;

namespace Lingua.API
{
    public class RoomUpdatesSignalRService :
        INotificationHandler<RoomEnteredEvent>,
        INotificationHandler<RoomLeftEvent>,
        INotificationHandler<RoomRemovedEvent>,
        INotificationHandler<RoomCreatedEvent>,
        INotificationHandler<RoomUpdatedEvent>,
        INotificationHandler<RoomMessageSentEvent>
    {
        private readonly IHubContext<RoomsHub, IRoomsRealtimeClient> _roomsHub;
        private readonly IMapper _mapper;

        public RoomUpdatesSignalRService(IHubContext<RoomsHub, IRoomsRealtimeClient> roomsHub, IMapper mapper)
        {
            _roomsHub = roomsHub;
            _mapper = mapper;
        }

        public Task Handle(RoomUpdatedEvent @event, CancellationToken cancellationToken)
        {
            var vm = _mapper.Map<RoomViewModel>(@event.Room);
            return _roomsHub.Clients.All.OnUpdate(vm, @event.User.Id);
        }

        public Task Handle(RoomCreatedEvent @event, CancellationToken cancellationToken)
        {
            var vm = _mapper.Map<RoomViewModel>(@event.Room);
            return _roomsHub.Clients.All.OnAdd(vm, @event.User.Id);
        }

        public Task Handle(RoomRemovedEvent @event, CancellationToken cancellationToken)
        {
            var vm = _mapper.Map<RoomViewModel>(@event.Room);
            return _roomsHub.Clients.All.OnRemove(vm, @event.User.Id);
        }

        public Task Handle(RoomLeftEvent @event, CancellationToken cancellationToken)
        {
            var vm = _mapper.Map<RoomViewModel>(@event.Room);
            return _roomsHub.Clients.All.OnLeave(vm, @event.User.Id);
        }

        public Task Handle(RoomEnteredEvent @event, CancellationToken cancellationToken)
        {
            var vm = _mapper.Map<RoomViewModel>(@event.Room);
            return _roomsHub.Clients.All.OnEnter(vm, @event.User.Id);
        }

        public Task Handle(RoomMessageSentEvent @event, CancellationToken cancellationToken)
        {
            var vm = _mapper.Map<RoomViewModel>(@event.Room);
            return _roomsHub.Clients.All.OnMessage(vm, @event.MessageId, @event.User.Id);
        }
    }
}
