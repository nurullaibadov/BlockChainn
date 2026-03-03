using Blockchain.Domain.Common;
using Blockchain.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blockchain.Domain.Entities
{
    public class SmartContract : BaseAuditableEntity
    {
        public Guid UserId { get; set; }
        public string ContractAddress { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Abi { get; set; } = string.Empty;
        public string? Bytecode { get; set; }
        public NetworkType Network { get; set; }
        public ContractStatus Status { get; set; } = ContractStatus.Active;
        public string? DeployerAddress { get; set; }
        public string? DeployTxHash { get; set; }
        public long? DeployBlockNumber { get; set; }
        public DateTime? DeployedAt { get; set; }

        // Navigation
        public AppUser User { get; set; } = null!;
        public ICollection<ContractInteraction> Interactions { get; set; } = new List<ContractInteraction>();
    }
}
