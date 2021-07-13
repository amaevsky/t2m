using System;

namespace Lingua.Shared
{
    public class CreateRoomOptions
    {
        public string Topic { get; set; }
        public DateTime StartDate { get; set; }
        public int DurationInMinutes { get; set; }
        public string Language { get; set; }
    }

}
