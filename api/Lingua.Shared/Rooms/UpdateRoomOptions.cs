using System;

namespace Lingua.Shared
{
    public class UpdateRoomOptions
    {
        public Guid RoomId { get; set; }
        public string Topic { get; set; }
        public DateTime? Date { get; set; }
        public TimeSpan? Duration { get; set; }
    }

}
