using Blockchain.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blockchain.Application.DTOs.Wallet
{
    public class WalletDto
    {
        public Guid Id { get; set; }
        public string Address { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public NetworkType Network { get; set; }
        public decimal Balance { get; set; }
        public bool IsDefault { get; set; }
        public WalletStatus Status { get; set; }
        public DateTime LastSyncedAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateWalletDto
    {
        public string Label { get; set; } = "My Wallet";
        public NetworkType Network { get; set; } = NetworkType.Ethereum;
        public bool IsDefault { get; set; } = false;
        public string? ImportPrivateKey { get; set; }
    }

    public class UpdateWalletDto
    {
        public string? Label { get; set; }
        public bool? IsDefault { get; set; }
    }
}
