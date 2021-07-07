using System;
using System.Collections.Generic;

namespace Lingua.ZoomIntegration
{
    public class Meeting
    {
        public string CreatedAt { get; set; }
        public int Duration { get; set; }
        public string HostId { get; set; }
        public string AssistantId { get; set; }
        public long Id { get; set; }
        public string JoinUrl { get; set; }
        public string StartTime { get; set; }
        public string StartUrl { get; set; }
        public string Status { get; set; }
        public string Timezone { get; set; }
        public string Topic { get; set; }
        public string Agenda { get; set; }
        public MeetingType Type { get; set; }
        public int Pmi { get; set; }
        public string Password { get; set; }
        public MeetingSettings Settings { get; set; }
    }

    public enum ReccurenceMeetingType
    {
        Daily = 1,
        Weekly = 2,
        Monthly = 3
    }

    public class CreateMeetingRecurrence
    {
        public ReccurenceMeetingType Type { get; set; }
        public int RepeatInterval { get; set; }
        public string WeeklyDays { get; set; }
        public int MonthlyDay { get; set; }
        public int MonthlyWeek { get; set; }
        public int MonthlyWeekDay { get; set; }
        public int EndTimes { get; set; }
        public string EndDateTime { get; set; }
    }

    public enum MeetingApprovalType
    {
        Auto = 0,
        Manually = 1,
        NoRegistration = 2
    }

    public enum MeetingRegistrationType
    {
        OnceForAll = 1,
        EveryTime = 2,
        OnceForSeveral = 3
    }

    public class MeetingSettings
    {
        public bool HostVideo { get; set; }
        public bool ParticipantVideo { get; set; }
        public bool CnMeeting { get; set; }
        public bool InMeeting { get; set; }
        public bool JoinBeforeHost { get; set; }
        public bool MuteUponEntry { get; set; }
        //public bool Watermark { get; set; }
        //public bool UsePmi { get; set; }
        //public MeetingApprovalType ApprovalType { get; set; }
        //public MeetingRegistrationType RegistrationType { get; set; }
        public string Audio { get; set; }
        //public string AutoRecording { get; set; }
        public bool EnforceLogin { get; set; }
        public string EnforceLoginDomains { get; set; }
        public string AlternativeHosts { get; set; }
        public IEnumerable<string> GlobalDialInCountries { get; set; }
        public bool RegistrantsEmailNotification { get; set; }
    }

    public enum MeetingType
    {
        Scheduled = 2,
        Instant = 1,
        ReccurinNoFixedTime = 3,
        ReccurinWithFixedTime = 4
    }

    public class CreateMeetingRequest
    {
        public string Topic { get; set; }
        public MeetingType Type { get; set; }
        public DateTime StartTime { get; set; }
        public int Duration { get; set; }
        public string ScheduleFor { get; set; }
        public string Timezone { get; set; }
        public string Password { get; set; }
        public string Agenda { get; set; }
        //public CreateMeetingRecurrence Recurrence { get; set; }
        public MeetingSettings Settings { get; set; }

    }
}
