using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blockchain.Domain.Enums
{
    public enum UserRole
    {
        SuperAdmin = 0,
        Admin = 1,
        User = 2
    }

    public enum NetworkType
    {
        Ethereum = 1,
        BSC = 2,
        Polygon = 3,
        Arbitrum = 4,
        Optimism = 5,
        Avalanche = 6
    }

    public enum WalletStatus
    {
        Active = 1,
        Frozen = 2,
        Deleted = 3
    }

    // "TransactionStatus" System.Transactions ile çakışıyor → TxStatus
    public enum TxStatus
    {
        Pending = 0,
        Processing = 1,
        Confirmed = 2,
        Failed = 3,
        Cancelled = 4
    }

    // "TransactionType" çakışma riski → TxType
    public enum TxType
    {
        Send = 1,
        Receive = 2,
        ContractDeploy = 3,
        ContractInteraction = 4,
        TokenTransfer = 5
    }

    public enum ContractStatus
    {
        Active = 1,
        Paused = 2,
        Destroyed = 3
    }

    public enum InteractionStatus
    {
        Pending = 0,
        Success = 1,
        Failed = 2
    }

    public enum NotificationType
    {
        TransactionConfirmed = 1,
        TransactionFailed = 2,
        LowBalance = 3,
        SecurityAlert = 4,
        SystemInfo = 5
    }
}
