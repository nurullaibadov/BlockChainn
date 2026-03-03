using Blockchain.Application.Common;
using Blockchain.Application.DTOs.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blockchain.Application.Interfaces
{
    public interface ITransactionService
    {
        Task<Result<TransactionDto>> SendTransactionAsync(Guid userId, SendTransactionDto dto, CancellationToken ct = default);
        Task<Result<TransactionDto>> GetTransactionByIdAsync(Guid txId, Guid userId, CancellationToken ct = default);
        Task<Result<TransactionDto>> GetTransactionByHashAsync(string txHash, CancellationToken ct = default);
        Task<Result<PagedResult<TransactionDto>>> GetUserTransactionsAsync(Guid userId, PaginationQuery query, CancellationToken ct = default);
        Task<Result<PagedResult<TransactionDto>>> GetWalletTransactionsAsync(Guid walletId, Guid userId, PaginationQuery query, CancellationToken ct = default);
        Task<Result> SyncTransactionStatusAsync(Guid txId, CancellationToken ct = default);
        Task SyncAllPendingTransactionsAsync(CancellationToken ct = default);
    }
}
