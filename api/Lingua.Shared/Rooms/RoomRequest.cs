namespace Lingua.Shared
{
    public abstract class RoomRequest : AuditableEntity
    {
        public User To { get; set; }
        public User From { get; set; }
        public RoomRequestStatus Status { get; set; }
    }

    public enum RoomRequestStatus
    {
        Requested,
        Accepted,
        Declined
    }

    public class EnterRoomRequest : RoomRequest
    {

    }
}
