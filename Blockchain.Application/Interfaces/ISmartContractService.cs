using Blockchain.Application.Common;
using Blockchain.Application.DTOs.Contract;
using Blockchain.Application.DTOs.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blockchain.Application.Interfaces
{

    public interface ISmartContractService
    {
        Task<Result<SmartContractDto>> AddContractAsync(Guid userId, AddContractDto dto, CancellationToken ct = default);
        Task<Result<SmartContractDto>> GetContractByIdAsync(Guid contractId, CancellationToken ct = default);
        Task<Result<PagedResult<SmartContractDto>>> GetContractsAsync(Guid userId, PaginationQuery query, CancellationToken ct = default);
        Task<Result<string>> CallContractFunctionAsync(Guid userId, CallContractDto dto, CancellationToken ct = default);
        Task<Result<TransactionDto>> SendContractTransactionAsync(Guid userId, SendContractTxDto dto, CancellationToken ct = default);
        Task<Result> DeleteContractAsync(Guid contractId, Guid userId, CancellationToken ct = default);
    }
}
