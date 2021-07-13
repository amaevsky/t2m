﻿using System;

namespace Lingua.Shared
{
    public class UpdateRoomOptions
    {
        public Guid RoomId { get; set; }
        public string Topic { get; set; }
        public DateTime? StartDate { get; set; }
        public int? DurationInMinutes { get; set; }
    }

}
