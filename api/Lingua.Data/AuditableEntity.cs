using System;

namespace Lingua.Data
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
        public bool IsRemoved { get; set; }
    }

}
