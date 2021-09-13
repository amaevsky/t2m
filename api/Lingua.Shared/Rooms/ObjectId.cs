using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Lingua.Shared
{
    [BsonIgnoreExtraElements]
    public class ObjectId : BaseEntity
    {
        public ObjectId(Guid id)
        {
            Id = id;
        }
    }
}