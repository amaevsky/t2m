using System;
using System.Collections.Generic;

namespace Lingua.Shared
{
    public class SearchRoomOptions
    {
        public List<string> Levels { get; set; }
        public List<DayOfWeek> Days { get; set; }
        public DateTime? TimeFrom { get; set; }
        public DateTime? TimeTo { get; set; }
    }

}
