using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace Lingua.Shared
{
    [BsonIgnoreExtraElements]
    public class RoomDto : BaseRoom
    {
        public RoomDto()
        {
            Participants = new List<ObjectId>();
        }

        public List<ObjectId> Participants { get; set; }
    }
}