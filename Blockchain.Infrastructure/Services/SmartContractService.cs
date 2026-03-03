using AutoMapper;
using Blockchain.Application.Common;
using Blockchain.Application.DTOs.Contract;
using Blockchain.Application.DTOs.Transaction;
using Blockchain.Application.Interfaces;
using Blockchain.Domain.Entities;
using Blockchain.Domain.Enums;
using Blockchain.Domain.Interfaces;
using DomainTransaction = Blockchain.Domain.Entities.Transaction;
using AppResult = Blockchain.Application.Common.Result;

namespace Blockchain.Infrastructure.Services
{
    public class SmartContractService : ISmartContractService
    {
        private readonly IUnitOfWork _uow;
        private readonly IBlockchainService _blockchain;
        private readonly IMapper _mapper;
        private readonly EncryptionService _encryption;

        public SmartContractService(IUnitOfWork uow, IBlockchainService blockchain, IMapper mapper, EncryptionService encryption)
        {
            _uow = uow;
            _blockchain = blockchain;
            _mapper = mapper;
            _encryption = encryption;
        }

        public async Task<Result<SmartContractDto>> AddContractAsync(Guid userId, AddContractDto dto, CancellationToken ct = default)
        {
            var existing = await _uow.SmartContracts.FirstOrDefaultAsync(c => c.ContractAddress == dto.ContractAddress && c.Network == dto.Network, ct);
            if (existing != null) return Result<SmartContractDto>.Failure("Contract already exists");

            var contract = _mapper.Map<SmartContract>(dto);
            contract.UserId = userId;

            await _uow.SmartContracts.AddAsync(contract, ct);
            await _uow.SaveChangesAsync(ct);
            return Result<SmartContractDto>.Success(_mapper.Map<SmartContractDto>(contract), null, 201);
        }

        public async Task<Result<SmartContractDto>> GetContractByIdAsync(Guid contractId, CancellationToken ct = default)
        {
            var contract = await _uow.SmartContracts.GetByIdAsync(contractId, ct);
            if (contract == null) return Result<SmartContractDto>.NotFound("Contract not found");
            return Result<SmartContractDto>.Success(_mapper.Map<SmartContractDto>(contract));
        }

        public async Task<Result<PagedResult<SmartContractDto>>> GetContractsAsync(Guid userId, PaginationQuery query, CancellationToken ct = default)
        {
            var (items, total) = await _uow.SmartContracts.GetPagedAsync(
                query.Page, query.PageSize,
                c => c.UserId == userId,
                q => q.OrderByDescending(c => c.CreatedAt), ct);

            return Result<PagedResult<SmartContractDto>>.Success(new PagedResult<SmartContractDto>
            {
                Items = _mapper.Map<IEnumerable<SmartContractDto>>(items),
                TotalCount = total,
                Page = query.Page,
                PageSize = query.PageSize
            });
        }

        public async Task<Result<string>> CallContractFunctionAsync(Guid userId, CallContractDto dto, CancellationToken ct = default)
        {
            var contract = await _uow.SmartContracts.GetByIdAsync(dto.ContractId, ct);
            if (contract == null) return Result<string>.NotFound("Contract not found");

            var interaction = new ContractInteraction
            {
                ContractId = contract.Id,
                UserId = userId,
                FunctionName = dto.FunctionName,
                InputData = System.Text.Json.JsonSerializer.Serialize(dto.Parameters),
                Status = InteractionStatus.Pending
            };

            try
            {
                var result = await _blockchain.CallContractFunctionAsync(
                    contract.ContractAddress, contract.Abi, dto.FunctionName, dto.Parameters, contract.Network);
                interaction.OutputData = result;
                interaction.Status = InteractionStatus.Success;
                await _uow.ContractInteractions.AddAsync(interaction, ct);
                await _uow.SaveChangesAsync(ct);
                return Result<string>.Success(result);
            }
            catch (Exception ex)
            {
                interaction.Status = InteractionStatus.Failed;
                interaction.ErrorMessage = ex.Message;
                await _uow.ContractInteractions.AddAsync(interaction, ct);
                await _uow.SaveChangesAsync(ct);
                return Result<string>.Failure($"Contract call failed: {ex.Message}");
            }
        }

        public async Task<Result<TransactionDto>> SendContractTransactionAsync(Guid userId, SendContractTxDto dto, CancellationToken ct = default)
        {
            var contract = await _uow.SmartContracts.GetByIdAsync(dto.ContractId, ct);
            if (contract == null) return Result<TransactionDto>.NotFound("Contract not found");

            var wallet = await _uow.Wallets.FirstOrDefaultAsync(w => w.Id == dto.FromWalletId && w.UserId == userId, ct);
            if (wallet == null) return Result<TransactionDto>.NotFound("Wallet not found");

            var privateKey = _encryption.Decrypt(wallet.EncryptedPrivateKey);
            var txHash = await _blockchain.SendContractTransactionAsync(
                privateKey, contract.ContractAddress, contract.Abi, dto.FunctionName, dto.Parameters, contract.Network);

            var tx = new DomainTransaction
            {
                UserId = userId,
                FromWalletId = wallet.Id,
                FromAddress = wallet.Address,
                ToAddress = contract.ContractAddress,
                Amount = 0,
                TxHash = txHash,
                Status = TxStatus.Pending,
                Type = TxType.ContractInteraction,
                Network = contract.Network
            };

            await _uow.Transactions.AddAsync(tx, ct);
            await _uow.SaveChangesAsync(ct);
            return Result<TransactionDto>.Success(_mapper.Map<TransactionDto>(tx));
        }

        public async Task<AppResult> DeleteContractAsync(Guid contractId, Guid userId, CancellationToken ct = default)
        {
            var contract = await _uow.SmartContracts.FirstOrDefaultAsync(c => c.Id == contractId && c.UserId == userId, ct);
            if (contract == null) return AppResult.Failure("Contract not found", 404);
            contract.IsDeleted = true;
            await _uow.SmartContracts.UpdateAsync(contract, ct);
            await _uow.SaveChangesAsync(ct);
            return AppResult.Success("Contract removed");
        }
    }
}
