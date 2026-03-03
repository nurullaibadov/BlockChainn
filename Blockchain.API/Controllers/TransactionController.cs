using Blockchain.Application.Common;
using Blockchain.Application.DTOs.Transaction;
using Blockchain.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Blockchain.API.Controllers
{
    /// <summary>Blockchain transactions</summary>
    [Tags("Transactions")]
    [Authorize]
    public class TransactionController : BaseApiController
    {
        private readonly ITransactionService _txService;

        public TransactionController(ITransactionService txService) => _txService = txService;

        /// <summary>Send ETH transaction</summary>
        [HttpPost("send")]
        public async Task<IActionResult> Send([FromBody] SendTransactionDto dto, CancellationToken ct)
            => HandleResult(await _txService.SendTransactionAsync(CurrentUserId, dto, ct));

        /// <summary>Get all my transactions (paginated)</summary>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginationQuery query, CancellationToken ct)
            => HandleResult(await _txService.GetUserTransactionsAsync(CurrentUserId, query, ct));

        /// <summary>Get transaction by ID</summary>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
            => HandleResult(await _txService.GetTransactionByIdAsync(id, CurrentUserId, ct));

        /// <summary>Get transaction by blockchain hash</summary>
        [HttpGet("hash/{txHash}")]
        public async Task<IActionResult> GetByHash(string txHash, CancellationToken ct)
            => HandleResult(await _txService.GetTransactionByHashAsync(txHash, ct));

        /// <summary>Get transactions for a specific wallet</summary>
        [HttpGet("wallet/{walletId:guid}")]
        public async Task<IActionResult> GetWalletTransactions(Guid walletId, [FromQuery] PaginationQuery query, CancellationToken ct)
            => HandleResult(await _txService.GetWalletTransactionsAsync(walletId, CurrentUserId, query, ct));

        /// <summary>Sync transaction status from blockchain</summary>
        [HttpPost("{id:guid}/sync")]
        public async Task<IActionResult> SyncStatus(Guid id, CancellationToken ct)
            => HandleResult(await _txService.SyncTransactionStatusAsync(id, ct));
    }
}
