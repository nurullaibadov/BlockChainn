using Blockchain.Application.Common;
using Blockchain.Application.DTOs.User;
using Blockchain.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Blockchain.API.Controllers
{
    /// <summary>Admin panel — full user management</summary>
    [Tags("Admin")]
    [Authorize]
    [Route("api/admin")]
    [ApiController]
    public class AdminController : BaseApiController
    {
        private readonly IUserService _userService;
        private readonly ITransactionService _txService;
        private readonly IWalletService _walletService;

        public AdminController(IUserService userService, ITransactionService txService, IWalletService walletService)
        {
            _userService = userService;
            _txService = txService;
            _walletService = walletService;
        }

        // ──── USERS ────────────────────────────────────────────────────────────

        /// <summary>Get all users (paginated + search)</summary>
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers([FromQuery] PaginationQuery query, CancellationToken ct)
            => HandleResult(await _userService.GetAllUsersAsync(query, ct));

        /// <summary>Get user by ID</summary>
        [HttpGet("users/{id:guid}")]
        public async Task<IActionResult> GetUser(Guid id, CancellationToken ct)
            => HandleResult(await _userService.GetUserByIdAsync(id, ct));

        /// <summary>Create user (admin)</summary>
        [HttpPost("users")]
        public async Task<IActionResult> CreateUser([FromBody] AdminCreateUserDto dto, CancellationToken ct)
            => HandleResult(await _userService.AdminCreateUserAsync(dto, ct));

        /// <summary>Update user</summary>
        [HttpPut("users/{id:guid}")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] AdminUpdateUserDto dto, CancellationToken ct)
            => HandleResult(await _userService.AdminUpdateUserAsync(id, dto, ct));

        /// <summary>Delete user</summary>
        [HttpDelete("users/{id:guid}")]
        public async Task<IActionResult> DeleteUser(Guid id, CancellationToken ct)
            => HandleResult(await _userService.AdminDeleteUserAsync(id, ct));

        /// <summary>Ban user</summary>
        [HttpPost("users/{id:guid}/ban")]
        public async Task<IActionResult> BanUser(Guid id, CancellationToken ct)
            => HandleResult(await _userService.BanUserAsync(id, ct));

        /// <summary>Unban user</summary>
        [HttpPost("users/{id:guid}/unban")]
        public async Task<IActionResult> UnbanUser(Guid id, CancellationToken ct)
            => HandleResult(await _userService.UnbanUserAsync(id, ct));

        // ──── TRANSACTIONS ──────────────────────────────────────────────────────

        /// <summary>Get transactions for a user</summary>
        [HttpGet("users/{userId:guid}/transactions")]
        public async Task<IActionResult> GetUserTransactions(Guid userId, [FromQuery] PaginationQuery query, CancellationToken ct)
            => HandleResult(await _txService.GetUserTransactionsAsync(userId, query, ct));

        // ──── WALLETS ───────────────────────────────────────────────────────────

        /// <summary>Get wallets for a user</summary>
        [HttpGet("users/{userId:guid}/wallets")]
        public async Task<IActionResult> GetUserWallets(Guid userId, [FromQuery] PaginationQuery query, CancellationToken ct)
            => HandleResult(await _walletService.GetUserWalletsAsync(userId, query, ct));

        // ──── SYSTEM ────────────────────────────────────────────────────────────

        /// <summary>Sync all pending transactions</summary>
        [HttpPost("sync-transactions")]
        public async Task<IActionResult> SyncTransactions(CancellationToken ct)
        {
            await _txService.SyncAllPendingTransactionsAsync(ct);
            return Ok(new { success = true, message = "Sync initiated" });
        }
    }
}
