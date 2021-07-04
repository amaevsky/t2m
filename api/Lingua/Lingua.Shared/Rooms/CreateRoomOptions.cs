using System;

namespace Lingua.Shared
{
    public class CreateRoomOptions
    {
        public string Topic { get; set; }
        public DateTime? Date { get; set; }
        public TimeSpan? Duration { get; set; }
        public string Language { get; set; }
        public User Host { get; set; }
    }

}
