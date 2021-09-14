using System;
using System.Collections.Generic;
using Lingua.Shared;

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
        public List<Message> Messages { get; set; }
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