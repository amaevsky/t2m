using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Lingua.Shared
{
    [BsonIgnoreExtraElements]
    public abstract class BaseEntity
    {
        protected BaseEntity()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }
    }

}
