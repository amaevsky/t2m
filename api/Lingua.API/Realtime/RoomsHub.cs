using Lingua.Shared;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace Lingua.API.Realtime
{

    public interface IRoomsRealtimeClient
    {
        Task OnAdd(Room room, Guid? by);
        Task OnUpdate(Room room, Guid? by);
        Task OnEnter(Room room, Guid? by);
        Task OnLeave(Room room, Guid? by);
        Task OnRemove(Room room, Guid? by);
    }

    public class RoomsHub : Hub<IRoomsRealtimeClient>
    {

    }
}


