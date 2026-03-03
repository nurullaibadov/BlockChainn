using AutoMapper;
using Blockchain.Application.Common;
using Blockchain.Application.DTOs.Transaction;
using Blockchain.Application.Interfaces;
using Blockchain.Domain.Enums;
using Blockchain.Domain.Interfaces;
using Blockchain.Infrastructure.Services;
using DomainTransaction = Blockchain.Domain.Entities.Transaction;
using AppResult = Blockchain.Application.Common.Result;

namespace Blockchain.Infrastructure.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IUnitOfWork _uow;
        private readonly IBlockchainService _blockchain;
        private readonly IMapper _mapper;
        private readonly EncryptionService _encryption;
        private readonly INotificationService _notification;
        private readonly IEmailService _emailService;

        public TransactionService(IUnitOfWork uow, IBlockchainService blockchain, IMapper mapper,
            EncryptionService encryption, INotificationService notification, IEmailService emailService)
        {
            _uow = uow;
            _blockchain = blockchain;
            _mapper = mapper;
            _encryption = encryption;
            _notification = notification;
            _emailService = emailService;
        }

        public async Task<Result<TransactionDto>> SendTransactionAsync(Guid userId, SendTransactionDto dto, CancellationToken ct = default)
        {
            var wallet = await _uow.Wallets.FirstOrDefaultAsync(w => w.Id == dto.FromWalletId && w.UserId == userId, ct);
            if (wallet == null) return Result<TransactionDto>.NotFound("Wallet not found");
            if (wallet.Status != WalletStatus.Active) return Result<TransactionDto>.Failure("Wallet is not active");
            if (wallet.Balance < dto.Amount) return Result<TransactionDto>.Failure("Insufficient balance");
            if (!await _blockchain.IsValidAddressAsync(dto.ToAddress))
                return Result<TransactionDto>.Failure("Invalid destination address");

            await _uow.BeginTransactionAsync(ct);
            try
            {
                var gasPrice = await _blockchain.GetGasPriceAsync(dto.Network);
                var fee = gasPrice * 21000 / 1_000_000_000m;

                var tx = new DomainTransaction
                {
                    UserId = userId,
                    FromWalletId = wallet.Id,
                    FromAddress = wallet.Address,
                    ToAddress = dto.ToAddress,
                    Amount = dto.Amount,
                    GasPrice = gasPrice,
                    Fee = fee,
                    Network = dto.Network,
                    Type = TxType.Send,
                    Status = TxStatus.Processing,
                    Notes = dto.Notes
                };

                await _uow.Transactions.AddAsync(tx, ct);
                await _uow.SaveChangesAsync(ct);

                var privateKey = _encryption.Decrypt(wallet.EncryptedPrivateKey);
                var txHash = await _blockchain.SendTransactionAsync(privateKey, dto.ToAddress, dto.Amount, dto.Network);

                tx.TxHash = txHash;
                tx.Status = TxStatus.Pending;
                wallet.Balance -= (dto.Amount + fee);
                wallet.UpdatedAt = DateTime.UtcNow;

                await _uow.Transactions.UpdateAsync(tx, ct);
                await _uow.Wallets.UpdateAsync(wallet, ct);
                await _uow.SaveChangesAsync(ct);
                await _uow.CommitTransactionAsync(ct);

                await _notification.CreateNotificationAsync(userId,
                    "Transaction Sent",
                    $"Transaction of {dto.Amount} ETH sent successfully. TxHash: {txHash}",
                    NotificationType.TransactionConfirmed, tx.Id.ToString(), ct);

                return Result<TransactionDto>.Success(_mapper.Map<TransactionDto>(tx), "Transaction sent");
            }
            catch (Exception ex)
            {
                await _uow.RollbackTransactionAsync(ct);
                return Result<TransactionDto>.Failure($"Transaction failed: {ex.Message}");
            }
        }

        public async Task<Result<TransactionDto>> GetTransactionByIdAsync(Guid txId, Guid userId, CancellationToken ct = default)
        {
            var tx = await _uow.Transactions.FirstOrDefaultAsync(t => t.Id == txId && t.UserId == userId, ct);
            if (tx == null) return Result<TransactionDto>.NotFound("Transaction not found");
            return Result<TransactionDto>.Success(_mapper.Map<TransactionDto>(tx));
        }

        public async Task<Result<TransactionDto>> GetTransactionByHashAsync(string txHash, CancellationToken ct = default)
        {
            var tx = await _uow.Transactions.FirstOrDefaultAsync(t => t.TxHash == txHash, ct);
            if (tx == null) return Result<TransactionDto>.NotFound("Transaction not found");
            return Result<TransactionDto>.Success(_mapper.Map<TransactionDto>(tx));
        }

        public async Task<Result<PagedResult<TransactionDto>>> GetUserTransactionsAsync(Guid userId, PaginationQuery query, CancellationToken ct = default)
        {
            var (items, total) = await _uow.Transactions.GetPagedAsync(
                query.Page, query.PageSize,
                t => t.UserId == userId,
                q => q.OrderByDescending(t => t.CreatedAt), ct);

            return Result<PagedResult<TransactionDto>>.Success(new PagedResult<TransactionDto>
            {
                Items = _mapper.Map<IEnumerable<TransactionDto>>(items),
                TotalCount = total,
                Page = query.Page,
                PageSize = query.PageSize
            });
        }

        public async Task<Result<PagedResult<TransactionDto>>> GetWalletTransactionsAsync(Guid walletId, Guid userId, PaginationQuery query, CancellationToken ct = default)
        {
            var wallet = await _uow.Wallets.FirstOrDefaultAsync(w => w.Id == walletId && w.UserId == userId, ct);
            if (wallet == null) return Result<PagedResult<TransactionDto>>.NotFound("Wallet not found");

            var (items, total) = await _uow.Transactions.GetPagedAsync(
                query.Page, query.PageSize,
                t => t.FromWalletId == walletId || t.ToWalletId == walletId,
                q => q.OrderByDescending(t => t.CreatedAt), ct);

            return Result<PagedResult<TransactionDto>>.Success(new PagedResult<TransactionDto>
            {
                Items = _mapper.Map<IEnumerable<TransactionDto>>(items),
                TotalCount = total,
                Page = query.Page,
                PageSize = query.PageSize
            });
        }

        public async Task<AppResult> SyncTransactionStatusAsync(Guid txId, CancellationToken ct = default)
        {
            var tx = await _uow.Transactions.GetByIdAsync(txId, ct);
            if (tx == null) return AppResult.Failure("Transaction not found", 404);
            if (string.IsNullOrEmpty(tx.TxHash)) return AppResult.Failure("No transaction hash");

            var receipt = await _blockchain.GetTransactionReceiptAsync(tx.TxHash, tx.Network) as Nethereum.RPC.Eth.DTOs.TransactionReceipt;
            if (receipt != null)
            {
                tx.Status = receipt.Status.Value == 1 ? TxStatus.Confirmed : TxStatus.Failed;
                tx.BlockHash = receipt.BlockHash;
                tx.BlockNumber = (long)receipt.BlockNumber.Value;
                tx.GasUsed = (decimal)receipt.GasUsed.Value;
                tx.ConfirmedAt = DateTime.UtcNow;
                await _uow.Transactions.UpdateAsync(tx, ct);
                await _uow.SaveChangesAsync(ct);
            }
            return AppResult.Success();
        }

        public async Task SyncAllPendingTransactionsAsync(CancellationToken ct = default)
        {
            var pending = await _uow.Transactions.FindAsync(
                t => t.Status == TxStatus.Pending && t.TxHash != null, ct);
            foreach (var tx in pending)
                await SyncTransactionStatusAsync(tx.Id, ct);
        }
    }
}
