using Blockchain.Application.Common;
using Blockchain.Application.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blockchain.Application.Interfaces
{
    public interface IAuthService
    {
        Task<Result<AuthResponseDto>> RegisterAsync(RegisterDto dto, CancellationToken ct = default);
        Task<Result<AuthResponseDto>> LoginAsync(LoginDto dto, CancellationToken ct = default);
        Task<Result<AuthResponseDto>> RefreshTokenAsync(RefreshTokenDto dto, CancellationToken ct = default);
        Task<Result> LogoutAsync(Guid userId, CancellationToken ct = default);
        Task<Result> ForgotPasswordAsync(ForgotPasswordDto dto, CancellationToken ct = default);
        Task<Result> ResetPasswordAsync(ResetPasswordDto dto, CancellationToken ct = default);
        Task<Result> ChangePasswordAsync(Guid userId, ChangePasswordDto dto, CancellationToken ct = default);
        Task<Result> VerifyEmailAsync(string email, string token, CancellationToken ct = default);
        Task<Result> ResendVerificationEmailAsync(string email, CancellationToken ct = default);
    }
}
