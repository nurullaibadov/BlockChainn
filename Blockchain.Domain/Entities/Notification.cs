using Blockchain.Domain.Common;
using Blockchain.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blockchain.Domain.Entities
{
    public class Notification : BaseEntity
    {
        public Guid UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public NotificationType Type { get; set; }
        public bool IsRead { get; set; } = false;
        public string? RelatedEntityId { get; set; }
        public string? RelatedEntityType { get; set; }

        // Navigation
        public AppUser User { get; set; } = null!;
    }
}
