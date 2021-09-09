using Lingua.Shared;
using MediatR;
using System;

namespace Lingua.Services.Rooms.Commands
{
    public class RoomCommand
    {
        public Guid UserId { get; set; }
        public Guid RoomId { get; set; }
    }

    public class BaseRoomCommand : RoomCommand, IRequest
    {

    }

    public class BaseRoomCommand<T> : RoomCommand, IRequest<T>
    {

    }
}
