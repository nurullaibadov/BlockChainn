using Blockchain.Domain.Common;
using Blockchain.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blockchain.Domain.Entities
{
    public class BlockchainTx : BaseAuditableEntity
    {
        public Guid UserId { get; set; }
        public Guid? FromWalletId { get; set; }
        public Guid? ToWalletId { get; set; }
        public string FromAddress { get; set; } = string.Empty;
        public string ToAddress { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public decimal GasPrice { get; set; }
        public decimal GasUsed { get; set; }
        public decimal Fee { get; set; }
        public string? TxHash { get; set; }
        public string? BlockHash { get; set; }
        public long? BlockNumber { get; set; }
        public TxStatus Status { get; set; } = TxStatus.Pending;
        public TxType Type { get; set; }
        public NetworkType Network { get; set; }
        public string? Notes { get; set; }
        public DateTime? ConfirmedAt { get; set; }
        public string? RawData { get; set; }

        // Navigation
        public AppUser User { get; set; } = null!;
        public Wallet? FromWallet { get; set; }
        public Wallet? ToWallet { get; set; }
    }

}
