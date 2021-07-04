using System;

namespace Lingua.Shared
{
    public class SearchRoomOptions
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public TimeSpan? Duration { get; set; }
        public string Language { get; set; }
    }

}
