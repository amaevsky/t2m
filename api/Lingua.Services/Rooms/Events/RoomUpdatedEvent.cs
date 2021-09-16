using Lingua.Services.Rooms.Queries;
using Lingua.Shared;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Lingua.Services.Rooms.Events
{
    public class RoomUpdatedEvent : BaseRoomEvent
    {
        public Room PreviousVersion { get; set; }
    }
}
