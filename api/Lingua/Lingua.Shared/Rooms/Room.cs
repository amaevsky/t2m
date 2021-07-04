using System;
using System.Collections.Generic;

namespace Lingua.Shared
{
    public class Room : AuditableEntity
    {
        public string Topic { get; set; }
        public DateTime? StartDate { get; set; }
        public TimeSpan? Duration { get; set; }
        public string Language { get; set; }
        public User Host { get; set; }
        public string JoinUrl { get; set; }
        public ICollection<User> Participants { get; set; }
    }
}
