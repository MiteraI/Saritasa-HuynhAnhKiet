using SecretsSharing.Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretsSharing.Domain.Entities
{
    public class Upload : AuditedBaseEntity<string>
    {
        public UploadTypes UploadType { get; set; }
        public string? FileName { get; set; }
        public string? MessageText { get; set; }
        public bool IsAutoDelete { get; set; } = false;
        public string UserId { get; set; }
        public virtual User User { get; set; }
    }
}
