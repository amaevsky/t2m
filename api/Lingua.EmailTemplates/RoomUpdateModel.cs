using Lingua.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lingua.EmailTemplates
{
    public class RoomUpdateModel : BaseModel
    {
        public Room Room { get; set; }
        public string Message { get; set; }
    }
}
