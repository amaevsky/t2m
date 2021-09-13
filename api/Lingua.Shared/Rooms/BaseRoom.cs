using System;
using System.Collections.Generic;

namespace Lingua.Shared
{
    public class BaseRoom : AuditableEntity
    {
        public BaseRoom()
        {
            Messages = new List<Message>();
        }
        public string Topic { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int DurationInMinutes { get; set; }
        public string Language { get; set; }
        public Guid HostUserId { get; set; }
        public string JoinUrl { get; set; }
        public int MaxParticipants { get; set; }
        public List<Message> Messages { get; }
    }
}