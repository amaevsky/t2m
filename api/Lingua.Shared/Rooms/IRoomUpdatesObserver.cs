using Lingua.Shared;
using System.Threading.Tasks;

namespace Lingua.Shared
{
    public interface IRoomUpdatesObserver
    {
        Task OnUpdate(RoomUpdateType updateType, Room room, User user);
    }

    public enum RoomUpdateType
    {
        Created,
        Updated,
        Entered,
        Left,
        Removed,
        Joined
    }
}
