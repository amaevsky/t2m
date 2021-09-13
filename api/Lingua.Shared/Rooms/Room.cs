using System;
using System.Collections.Generic;

namespace Lingua.Shared
{
    public class Room : BaseRoom
    {
        public Room()
        {
            Participants = new List<User>();
        }

        public List<User> Participants { get; set; }

        public User User(Guid userId) => Participants.Find(p => p.Id == userId);
        public bool IsFull => Participants.Count == MaxParticipants;
    }
}
