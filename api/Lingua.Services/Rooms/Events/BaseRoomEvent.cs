using Lingua.Shared;
using MediatR;
using System;

namespace Lingua.Services.Rooms.Events
{
    public class BaseRoomEvent : INotification
    {
        public Room Room { get; set; }
        public Guid UserId { get; set; }
    }
}
