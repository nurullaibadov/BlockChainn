using Blockchain.Application.Common;
using Blockchain.Application.DTOs.Wallet;
using Blockchain.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Blockchain.API.Controllers
{
    /// <summary>Wallet management</summary>
    [Tags("Wallets")]
    [Authorize]
    public class WalletController : BaseApiController
    {
        private readonly IWalletService _walletService;

        public WalletController(IWalletService walletService) => _walletService = walletService;

        /// <summary>Create or import a wallet</summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateWalletDto dto, CancellationToken ct)
            => HandleResult(await _walletService.CreateWalletAsync(CurrentUserId, dto, ct));

        /// <summary>Get all wallets (paginated)</summary>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginationQuery query, CancellationToken ct)
            => HandleResult(await _walletService.GetUserWalletsAsync(CurrentUserId, query, ct));

        /// <summary>Get wallet by ID</summary>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
            => HandleResult(await _walletService.GetWalletByIdAsync(id, CurrentUserId, ct));

        /// <summary>Update wallet</summary>
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateWalletDto dto, CancellationToken ct)
            => HandleResult(await _walletService.UpdateWalletAsync(id, CurrentUserId, dto, ct));

        /// <summary>Delete wallet</summary>
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
            => HandleResult(await _walletService.DeleteWalletAsync(id, CurrentUserId, ct));

        /// <summary>Sync wallet balance from blockchain</summary>
        [HttpPost("{id:guid}/sync-balance")]
        public async Task<IActionResult> SyncBalance(Guid id, CancellationToken ct)
            => HandleResult(await _walletService.SyncBalanceAsync(id, CurrentUserId, ct));

        /// <summary>Set as default wallet</summary>
        [HttpPost("{id:guid}/set-default")]
        public async Task<IActionResult> SetDefault(Guid id, CancellationToken ct)
            => HandleResult(await _walletService.SetDefaultWalletAsync(id, CurrentUserId, ct));

        /// <summary>Export private key (use with caution!)</summary>
        [HttpPost("{id:guid}/export-private-key")]
        public async Task<IActionResult> ExportPrivateKey(Guid id, [FromQuery] string password, CancellationToken ct)
            => HandleResult(await _walletService.ExportPrivateKeyAsync(id, CurrentUserId, password, ct));
    }
}
