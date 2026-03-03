using AutoMapper;
using Blockchain.Application.Common;
using Blockchain.Application.DTOs.Auth;
using Blockchain.Application.Interfaces;
using Blockchain.Domain.Entities;
using Blockchain.Domain.Enums;
using Blockchain.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using AppResult = Blockchain.Application.Common.Result;

namespace Blockchain.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly IAuditService _auditService;

        public AuthService(
            UserManager<AppUser> userManager,
            ITokenService tokenService,
            IEmailService emailService,
            IMapper mapper,
            IConfiguration config,
            IAuditService auditService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _emailService = emailService;
            _mapper = mapper;
            _config = config;
            _auditService = auditService;
        }

        public async Task<Result<AuthResponseDto>> RegisterAsync(RegisterDto dto, CancellationToken ct = default)
        {
            if (await _userManager.FindByEmailAsync(dto.Email) != null)
                return Result<AuthResponseDto>.Failure("Email already in use");

            if (await _userManager.FindByNameAsync(dto.UserName) != null)
                return Result<AuthResponseDto>.Failure("Username already taken");

            var user = new AppUser
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                UserName = dto.UserName,
                PhoneNumber = dto.PhoneNumber,
                Role = UserRole.User,
                EmailVerificationToken = Guid.NewGuid().ToString("N")
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                return Result<AuthResponseDto>.Failure(result.Errors.Select(e => e.Description).ToList());

            await _emailService.SendWelcomeEmailAsync(user.Email!, user.FirstName, ct);
            await _emailService.SendEmailConfirmationAsync(user.Email!, user.EmailVerificationToken!, ct);
            await _auditService.LogAsync("Register", "AppUser", user.Id.ToString(), isSuccess: true);

            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(
                double.Parse(_config["JwtSettings:RefreshTokenExpirationDays"]!));
            await _userManager.UpdateAsync(user);

            return Result<AuthResponseDto>.Success(new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                AccessTokenExpiry = DateTime.UtcNow.AddMinutes(
                    double.Parse(_config["JwtSettings:AccessTokenExpirationMinutes"]!)),
                RefreshTokenExpiry = user.RefreshTokenExpiry!.Value,
                User = _mapper.Map<UserInfoDto>(user)
            }, "Registration successful", 201);
        }

        public async Task<Result<AuthResponseDto>> LoginAsync(LoginDto dto, CancellationToken ct = default)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
            {
                await _auditService.LogAsync("Login", "AppUser", null, isSuccess: false,
                    errorMessage: "Invalid credentials");
                return Result<AuthResponseDto>.Failure("Invalid email or password", 401);
            }

            if (!user.IsActive)
                return Result<AuthResponseDto>.Failure("Account is deactivated", 403);

            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(
                double.Parse(_config["JwtSettings:RefreshTokenExpirationDays"]!));
            await _userManager.UpdateAsync(user);
            await _auditService.LogAsync("Login", "AppUser", user.Id.ToString(), isSuccess: true);

            return Result<AuthResponseDto>.Success(new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                AccessTokenExpiry = DateTime.UtcNow.AddMinutes(
                    double.Parse(_config["JwtSettings:AccessTokenExpirationMinutes"]!)),
                RefreshTokenExpiry = user.RefreshTokenExpiry!.Value,
                User = _mapper.Map<UserInfoDto>(user)
            });
        }

        public async Task<Result<AuthResponseDto>> RefreshTokenAsync(RefreshTokenDto dto, CancellationToken ct = default)
        {
            var principal = _tokenService.GetPrincipalFromExpiredToken(dto.AccessToken);
            if (principal == null)
                return Result<AuthResponseDto>.Failure("Invalid access token", 401);

            var userId = principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var user = await _userManager.FindByIdAsync(userId!);

            if (user == null || user.RefreshToken != dto.RefreshToken || user.RefreshTokenExpiry < DateTime.UtcNow)
                return Result<AuthResponseDto>.Failure("Invalid or expired refresh token", 401);

            var newAccessToken = _tokenService.GenerateAccessToken(user);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(
                double.Parse(_config["JwtSettings:RefreshTokenExpirationDays"]!));
            await _userManager.UpdateAsync(user);

            return Result<AuthResponseDto>.Success(new AuthResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                AccessTokenExpiry = DateTime.UtcNow.AddMinutes(
                    double.Parse(_config["JwtSettings:AccessTokenExpirationMinutes"]!)),
                RefreshTokenExpiry = user.RefreshTokenExpiry!.Value,
                User = _mapper.Map<UserInfoDto>(user)
            });
        }

        public async Task<AppResult> LogoutAsync(Guid userId, CancellationToken ct = default)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return AppResult.Failure("User not found", 404);
            user.RefreshToken = null;
            user.RefreshTokenExpiry = null;
            await _userManager.UpdateAsync(user);
            return AppResult.Success("Logged out");
        }

        public async Task<AppResult> ForgotPasswordAsync(ForgotPasswordDto dto, CancellationToken ct = default)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null) return AppResult.Success("If email exists, link has been sent");
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            user.PasswordResetToken = token;
            user.PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(1);
            await _userManager.UpdateAsync(user);
            await _emailService.SendPasswordResetAsync(user.Email!, token, ct);
            return AppResult.Success("Password reset link sent");
        }

        public async Task<AppResult> ResetPasswordAsync(ResetPasswordDto dto, CancellationToken ct = default)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null) return AppResult.Failure("Invalid request");
            if (user.PasswordResetTokenExpiry < DateTime.UtcNow)
                return AppResult.Failure("Token expired");
            var result = await _userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword);
            if (!result.Succeeded) return AppResult.Failure(result.Errors.First().Description);
            user.PasswordResetToken = null;
            user.PasswordResetTokenExpiry = null;
            await _userManager.UpdateAsync(user);
            return AppResult.Success("Password reset successfully");
        }

        public async Task<AppResult> ChangePasswordAsync(Guid userId, ChangePasswordDto dto, CancellationToken ct = default)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return AppResult.Failure("User not found", 404);
            var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
            if (!result.Succeeded) return AppResult.Failure(result.Errors.First().Description);
            return AppResult.Success("Password changed");
        }

        public async Task<AppResult> VerifyEmailAsync(string email, string token, CancellationToken ct = default)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return AppResult.Failure("User not found", 404);
            if (user.EmailVerified) return AppResult.Success("Already verified");
            if (user.EmailVerificationToken != token) return AppResult.Failure("Invalid token");
            user.EmailVerified = true;
            user.EmailVerificationToken = null;
            user.EmailConfirmed = true;
            await _userManager.UpdateAsync(user);
            return AppResult.Success("Email verified");
        }

        public async Task<AppResult> ResendVerificationEmailAsync(string email, CancellationToken ct = default)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return AppResult.Success("If email exists, link has been sent");
            if (user.EmailVerified) return AppResult.Failure("Already verified");
            user.EmailVerificationToken = Guid.NewGuid().ToString("N");
            await _userManager.UpdateAsync(user);
            await _emailService.SendEmailConfirmationAsync(email, user.EmailVerificationToken, ct);
            return AppResult.Success("Verification email sent");
        }
    }
}
