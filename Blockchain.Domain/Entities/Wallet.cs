using Blockchain.Domain.Common;
using Blockchain.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blockchain.Domain.Entities
{
    public class Wallet : BaseAuditableEntity
    {
        public Guid UserId { get; set; }
        public string Address { get; set; } = string.Empty;
        public string EncryptedPrivateKey { get; set; } = string.Empty;
        public string PublicKey { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public NetworkType Network { get; set; } = NetworkType.Ethereum;
        public decimal Balance { get; set; } = 0;
        public DateTime LastSyncedAt { get; set; } = DateTime.UtcNow;
        public bool IsDefault { get; set; } = false;
        public WalletStatus Status { get; set; } = WalletStatus.Active;

        // Navigation
        public AppUser User { get; set; } = null!;
        public ICollection<Transaction> SentTransactions { get; set; } = new List<Transaction>();
        public ICollection<Transaction> ReceivedTransactions { get; set; } = new List<Transaction>();
    }
}
