using AutoMapper;
using Blockchain.Application.Common;
using Blockchain.Application.DTOs.Wallet;
using Blockchain.Application.Interfaces;
using Blockchain.Domain.Entities;
using Blockchain.Domain.Enums;
using Blockchain.Domain.Interfaces;
using Nethereum.Hex.HexConvertors.Extensions;
using AppResult = Blockchain.Application.Common.Result;

namespace Blockchain.Infrastructure.Services
{
    public class WalletService : IWalletService
    {
        private readonly IUnitOfWork _uow;
        private readonly IBlockchainService _blockchain;
        private readonly IMapper _mapper;
        private readonly EncryptionService _encryption;
        private readonly IAuditService _auditService;

        public WalletService(IUnitOfWork uow, IBlockchainService blockchain, IMapper mapper,
            EncryptionService encryption, IAuditService auditService)
        {
            _uow = uow;
            _blockchain = blockchain;
            _mapper = mapper;
            _encryption = encryption;
            _auditService = auditService;
        }

        public async Task<Result<WalletDto>> CreateWalletAsync(Guid userId, CreateWalletDto dto, CancellationToken ct = default)
        {
            string address, privateKey, publicKey;

            if (!string.IsNullOrEmpty(dto.ImportPrivateKey))
            {
                var ecKey = new Nethereum.Signer.EthECKey(dto.ImportPrivateKey);
                address = ecKey.GetPublicAddress();
                privateKey = dto.ImportPrivateKey;
                publicKey = ecKey.GetPubKeyNoPrefix().ToHex(true);
            }
            else
            {
                (address, privateKey, publicKey) = await _blockchain.CreateWalletAsync();
            }

            var existing = await _uow.Wallets.FirstOrDefaultAsync(w => w.Address == address && w.UserId == userId, ct);
            if (existing != null)
                return Result<WalletDto>.Failure("Wallet with this address already exists");

            if (dto.IsDefault)
            {
                var currentDefault = await _uow.Wallets.FirstOrDefaultAsync(w => w.UserId == userId && w.IsDefault, ct);
                if (currentDefault != null)
                {
                    currentDefault.IsDefault = false;
                    await _uow.Wallets.UpdateAsync(currentDefault, ct);
                }
            }

            var wallet = new Wallet
            {
                UserId = userId,
                Address = address,
                EncryptedPrivateKey = _encryption.Encrypt(privateKey),
                PublicKey = publicKey,
                Label = dto.Label,
                Network = dto.Network,
                IsDefault = dto.IsDefault
            };

            await _uow.Wallets.AddAsync(wallet, ct);
            await _uow.SaveChangesAsync(ct);

            var balance = await _blockchain.GetBalanceAsync(address, dto.Network);
            wallet.Balance = balance;
            await _uow.Wallets.UpdateAsync(wallet, ct);
            await _uow.SaveChangesAsync(ct);

            await _auditService.LogAsync("CreateWallet", "Wallet", wallet.Id.ToString(), newValues: new { address, network = dto.Network });
            return Result<WalletDto>.Success(_mapper.Map<WalletDto>(wallet), "Wallet created", 201);
        }

        public async Task<Result<WalletDto>> GetWalletByIdAsync(Guid walletId, Guid userId, CancellationToken ct = default)
        {
            var wallet = await _uow.Wallets.FirstOrDefaultAsync(w => w.Id == walletId && w.UserId == userId, ct);
            if (wallet == null) return Result<WalletDto>.NotFound("Wallet not found");
            return Result<WalletDto>.Success(_mapper.Map<WalletDto>(wallet));
        }

        public async Task<Result<PagedResult<WalletDto>>> GetUserWalletsAsync(Guid userId, PaginationQuery query, CancellationToken ct = default)
        {
            var (items, total) = await _uow.Wallets.GetPagedAsync(
                query.Page, query.PageSize,
                w => w.UserId == userId && !w.IsDeleted,
                q => q.OrderByDescending(w => w.CreatedAt), ct);

            return Result<PagedResult<WalletDto>>.Success(new PagedResult<WalletDto>
            {
                Items = _mapper.Map<IEnumerable<WalletDto>>(items),
                TotalCount = total,
                Page = query.Page,
                PageSize = query.PageSize
            });
        }

        public async Task<Result<WalletDto>> UpdateWalletAsync(Guid walletId, Guid userId, UpdateWalletDto dto, CancellationToken ct = default)
        {
            var wallet = await _uow.Wallets.FirstOrDefaultAsync(w => w.Id == walletId && w.UserId == userId, ct);
            if (wallet == null) return Result<WalletDto>.NotFound("Wallet not found");

            if (dto.Label != null) wallet.Label = dto.Label;
            if (dto.IsDefault == true)
            {
                var currentDefault = await _uow.Wallets.FirstOrDefaultAsync(w => w.UserId == userId && w.IsDefault && w.Id != walletId, ct);
                if (currentDefault != null) { currentDefault.IsDefault = false; await _uow.Wallets.UpdateAsync(currentDefault, ct); }
                wallet.IsDefault = true;
            }

            await _uow.Wallets.UpdateAsync(wallet, ct);
            await _uow.SaveChangesAsync(ct);
            return Result<WalletDto>.Success(_mapper.Map<WalletDto>(wallet));
        }

        public async Task<AppResult> DeleteWalletAsync(Guid walletId, Guid userId, CancellationToken ct = default)
        {
            var wallet = await _uow.Wallets.FirstOrDefaultAsync(w => w.Id == walletId && w.UserId == userId, ct);
            if (wallet == null) return AppResult.Failure("Wallet not found", 404);
            wallet.IsDeleted = true;
            wallet.Status = WalletStatus.Deleted;
            await _uow.Wallets.UpdateAsync(wallet, ct);
            await _uow.SaveChangesAsync(ct);
            return AppResult.Success("Wallet deleted");
        }

        public async Task<Result<decimal>> SyncBalanceAsync(Guid walletId, Guid userId, CancellationToken ct = default)
        {
            var wallet = await _uow.Wallets.FirstOrDefaultAsync(w => w.Id == walletId && w.UserId == userId, ct);
            if (wallet == null) return Result<decimal>.NotFound("Wallet not found");
            wallet.Balance = await _blockchain.GetBalanceAsync(wallet.Address, wallet.Network);
            wallet.LastSyncedAt = DateTime.UtcNow;
            await _uow.Wallets.UpdateAsync(wallet, ct);
            await _uow.SaveChangesAsync(ct);
            return Result<decimal>.Success(wallet.Balance);
        }

        public async Task<AppResult> SetDefaultWalletAsync(Guid walletId, Guid userId, CancellationToken ct = default)
        {
            var wallets = await _uow.Wallets.FindAsync(w => w.UserId == userId, ct);
            foreach (var w in wallets) { w.IsDefault = w.Id == walletId; await _uow.Wallets.UpdateAsync(w, ct); }
            await _uow.SaveChangesAsync(ct);
            return AppResult.Success("Default wallet updated");
        }

        public async Task<Result<string>> ExportPrivateKeyAsync(Guid walletId, Guid userId, string password, CancellationToken ct = default)
        {
            var wallet = await _uow.Wallets.FirstOrDefaultAsync(w => w.Id == walletId && w.UserId == userId, ct);
            if (wallet == null) return Result<string>.NotFound("Wallet not found");
            var privateKey = _encryption.Decrypt(wallet.EncryptedPrivateKey);
            await _auditService.LogAsync("ExportPrivateKey", "Wallet", walletId.ToString(), isSuccess: true);
            return Result<string>.Success(privateKey);
        }
    }
}
