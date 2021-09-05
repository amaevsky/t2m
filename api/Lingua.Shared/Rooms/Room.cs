using System;
using System.Collections.Generic;

namespace Lingua.Shared
{
    public class Room : AuditableEntity
    {
        public Room()
        {
            Participants = new List<RoomParticipant>();
            Requests = new List<RoomRequest>();
        }

        public string Topic { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int DurationInMinutes { get; set; }
        public string Language { get; set; }
        public Guid HostUserId { get; set; }
        public string JoinUrl { get; set; }
        public int MaxParticipants { get; set; }
        public List<RoomParticipant> Participants { get; set; }
        public List<RoomRequest> Requests { get; set; }

        public RoomParticipant Host => Participants.Find(p => p.Id == HostUserId);
    }
}
