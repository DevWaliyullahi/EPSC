using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPSC.Domain.Entities.TransactionLog
{
    public class TTransactionLog : BaseEntity
    {
        public Guid TransactionLogId { get; set; }
        public string Action { get; set; } = string.Empty;
        public Guid EntityId { get; set; }
        public string? EntityType { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string? UserId { get; set; }
        public string? Details { get; set; }
        public string? OldValues { get; set; }
        public string? NewValues { get; set; }
    }
}
