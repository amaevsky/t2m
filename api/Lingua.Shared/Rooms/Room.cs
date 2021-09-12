using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace Lingua.Shared
{
    public class BaseRoom : AuditableEntity
    {
        public string Topic { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int DurationInMinutes { get; set; }
        public string Language { get; set; }
        public Guid HostUserId { get; set; }
        public string JoinUrl { get; set; }
        public int MaxParticipants { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class RoomDTO : BaseRoom
    {
        public RoomDTO()
        {
            Participants = new List<ObjectId>();
        }

        public List<ObjectId> Participants { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class ObjectId : BaseEntity
    {
        public ObjectId(Guid id)
        {
            Id = id;
        }
    }

    public class Room : BaseRoom
    {
        public Room()
        {
            Participants = new List<User>();
        }

        public List<User> Participants { get; set; }

        public User User(Guid userId) => Participants.Find(p => p.Id == userId);
        public bool IsFull => Participants.Count == MaxParticipants;
    }
}
