using Lingua.API.ViewModels;
using Lingua.Shared;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace Lingua.API.Realtime
{
    public interface IRoomsRealtimeClient
    {
        Task OnAdd(RoomViewModel room, Guid? by);
        Task OnUpdate(RoomViewModel room, Guid? by);
        Task OnEnter(RoomViewModel room, Guid? by);
        Task OnLeave(RoomViewModel room, Guid? by);
        Task OnRemove(RoomViewModel room, Guid? by);
        Task OnMessage(RoomViewModel room, Guid messageId, Guid? by);
    }

    public class RoomsHub : Hub<IRoomsRealtimeClient>
    {
    }
}