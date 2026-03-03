using Blockchain.Domain.Enums;

namespace Blockchain.Application.DTOs.Transaction
{
    public class TransactionDto
    {
        public Guid Id { get; set; }
        public string FromAddress { get; set; } = string.Empty;
        public string ToAddress { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public decimal Fee { get; set; }
        public string? TxHash { get; set; }
        public long? BlockNumber { get; set; }
        public TxStatus Status { get; set; }
        public TxType Type { get; set; }
        public NetworkType Network { get; set; }
        public string? Notes { get; set; }
        public DateTime? ConfirmedAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class SendTransactionDto
    {
        public Guid FromWalletId { get; set; }
        public string ToAddress { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public NetworkType Network { get; set; } = NetworkType.Ethereum;
        public string? Notes { get; set; }
        public decimal? CustomGasPrice { get; set; }
    }
}
