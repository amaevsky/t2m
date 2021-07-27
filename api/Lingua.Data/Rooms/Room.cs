using System;
using System.Collections.Generic;

namespace Lingua.Data
{
    public class Room : AuditableEntity
    {
        public Room()
        {
            Participants = new List<User>();
        }

        public string Topic { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int DurationInMinutes { get; set; }
        public string Language { get; set; }
        public Guid HostUserId { get; set; }
        public string JoinUrl { get; set; }
        public int MaxParticipants { get; set; }
        public List<User> Participants { get; set; }

        public User Host => Participants.Find(p => p.Id == HostUserId);
    }
}
