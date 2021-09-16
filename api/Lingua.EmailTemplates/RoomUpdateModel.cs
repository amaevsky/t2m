using Lingua.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lingua.EmailTemplates
{
    public class RoomUpdateModel : RoomModel
    {
        public string Message { get; set; }
        public Room PreviousVersion { get; set; }
    }

    public class RoomModel : BaseModel
    {
        public Room Room { get; set; }
    }

    public class UnreadRoomMessageModel : RoomModel
    {
        public User Author { get; set; }
    }
}
