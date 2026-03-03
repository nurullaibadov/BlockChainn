using Blockchain.Domain.Common;
using Blockchain.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blockchain.Domain.Entities
{
    public class ContractInteraction : BaseAuditableEntity
    {
        public Guid ContractId { get; set; }
        public Guid UserId { get; set; }
        public string FunctionName { get; set; } = string.Empty;
        public string? InputData { get; set; }
        public string? OutputData { get; set; }
        public string? TxHash { get; set; }
        public InteractionStatus Status { get; set; } = InteractionStatus.Pending;
        public decimal GasUsed { get; set; }
        public string? ErrorMessage { get; set; }

        // Navigation
        public SmartContract Contract { get; set; } = null!;
        public AppUser User { get; set; } = null!;
    }
}
