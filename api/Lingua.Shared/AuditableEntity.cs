using System;

namespace Lingua.Shared
{
    public abstract class AuditableEntity : BaseEntity
    {
        protected AuditableEntity()
        {
            var now = DateTime.UtcNow;
            Created = now;
            Updated = now;
        }

        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
    }

}
