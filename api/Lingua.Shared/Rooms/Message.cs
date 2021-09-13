using System;

namespace Lingua.Shared
{
    public class Message : AuditableEntity
    {
        public string Content { get; set; }
        public Guid AuthorId { get; set; }
    }
}