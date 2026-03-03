using Blockchain.Application.Common;
using Blockchain.Application.DTOs.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blockchain.Application.Interfaces
{
    public interface IWalletService
    {
        Task<Result<WalletDto>> CreateWalletAsync(Guid userId, CreateWalletDto dto, CancellationToken ct = default);
        Task<Result<WalletDto>> GetWalletByIdAsync(Guid walletId, Guid userId, CancellationToken ct = default);
        Task<Result<PagedResult<WalletDto>>> GetUserWalletsAsync(Guid userId, PaginationQuery query, CancellationToken ct = default);
        Task<Result<WalletDto>> UpdateWalletAsync(Guid walletId, Guid userId, UpdateWalletDto dto, CancellationToken ct = default);
        Task<Result> DeleteWalletAsync(Guid walletId, Guid userId, CancellationToken ct = default);
        Task<Result<decimal>> SyncBalanceAsync(Guid walletId, Guid userId, CancellationToken ct = default);
        Task<Result> SetDefaultWalletAsync(Guid walletId, Guid userId, CancellationToken ct = default);
        Task<Result<string>> ExportPrivateKeyAsync(Guid walletId, Guid userId, string password, CancellationToken ct = default);
    }

}
