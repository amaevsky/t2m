using Lingua.Shared;
using System;
using System.Collections.Generic;

namespace Lingua.API.ViewModels
{
    public class RoomViewModel
    {
        public Guid Id { get; set; }
        public string Topic { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int DurationInMinutes { get; set; }
        public string Language { get; set; }
        public Guid HostUserId { get; set; }
        public int MaxParticipants { get; set; }
        public List<RoomUserViewModel> Participants { get; set; }
    }

    public class RoomRequestViewModel
    {
        public Guid Id { get; set; }
        public RoomUserViewModel To { get; set; }
        public RoomUserViewModel From { get; set; }
        public RoomViewModel Room { get; set; }
        public RoomRequestStatus Status { get; set; }
    }

    public class RoomUserViewModel
    {
        public Guid Id { get; set; }
        public string Lastname { get; set; }
        public string Firstname { get; set; }
        public string TargetLanguage { get; set; }
        public string LanguageLevel { get; set; }
        public string AvatarUrl { get; set; }
    }
}
