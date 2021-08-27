using System;
using System.Collections.Generic;
using System.Linq;

namespace Lingua.Shared
{
    public class Room : AuditableEntity
    {
        public Room()
        {
            Participants = new List<RoomParticipant>();
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

        public RoomParticipant Host => Participants.Find(p => p.Id == HostUserId);
    }

    public class RoomParticipant : User
    {
        public RoomParticipant()
        {
        }

        public RoomParticipant(User user)
        {
            Id = user.Id;
            Lastname = user.Lastname;
            Firstname = user.Firstname;
            Email = user.Email;
            Country = user.Country;
            DateOfBirth = user.DateOfBirth;
            TargetLanguage = user.TargetLanguage;
            LanguageLevel = user.LanguageLevel;
            AvatarUrl = user.AvatarUrl;
            Timezone = user.Timezone;
        }

        public ParticipantStatus Status { get; set; }
    }

    public enum ParticipantStatus
    {
        Accepted,
        Requested,
        Declined
    }
}
