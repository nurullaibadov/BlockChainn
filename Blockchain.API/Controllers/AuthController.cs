using Blockchain.Application.DTOs.Auth;
using Blockchain.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Blockchain.API.Controllers
{
    /// <summary>Authentication endpoints</summary>
    [Tags("Authentication")]
    public class AuthController : BaseApiController
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService) => _authService = authService;

        /// <summary>Register new user</summary>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto, CancellationToken ct)
            => HandleResult(await _authService.RegisterAsync(dto, ct));

        /// <summary>Login with email and password</summary>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginDto dto, CancellationToken ct)
            => HandleResult(await _authService.LoginAsync(dto, ct));

        /// <summary>Refresh access token</summary>
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto dto, CancellationToken ct)
            => HandleResult(await _authService.RefreshTokenAsync(dto, ct));

        /// <summary>Logout current user</summary>
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout(CancellationToken ct)
            => HandleResult(await _authService.LogoutAsync(CurrentUserId, ct));

        /// <summary>Request password reset email</summary>
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto, CancellationToken ct)
            => HandleResult(await _authService.ForgotPasswordAsync(dto, ct));

        /// <summary>Reset password with token</summary>
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto, CancellationToken ct)
            => HandleResult(await _authService.ResetPasswordAsync(dto, ct));

        /// <summary>Change password (authenticated)</summary>
        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto, CancellationToken ct)
            => HandleResult(await _authService.ChangePasswordAsync(CurrentUserId, dto, ct));

        /// <summary>Verify email address</summary>
        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromQuery] string email, [FromQuery] string token, CancellationToken ct)
            => HandleResult(await _authService.VerifyEmailAsync(email, token, ct));

        /// <summary>Resend verification email</summary>
        [HttpPost("resend-verification")]
        public async Task<IActionResult> ResendVerification([FromBody] ForgotPasswordDto dto, CancellationToken ct)
            => HandleResult(await _authService.ResendVerificationEmailAsync(dto.Email, ct));
    }
}
