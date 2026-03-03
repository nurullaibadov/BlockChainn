using AutoMapper;
using Blockchain.Application.Common;
using Blockchain.Application.DTOs.User;
using Blockchain.Application.Interfaces;
using Blockchain.Domain.Entities;
using Blockchain.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AppResult = Blockchain.Application.Common.Result;

namespace Blockchain.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IAuditService _auditService;

        public UserService(UserManager<AppUser> userManager, IMapper mapper, IAuditService auditService)
        {
            _userManager = userManager;
            _mapper = mapper;
            _auditService = auditService;
        }

        public async Task<Result<UserDto>> GetProfileAsync(Guid userId, CancellationToken ct = default)
        {
            var user = await _userManager.Users
                .Include(u => u.Wallets)
                .Include(u => u.Transactions)
                .FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return Result<UserDto>.NotFound("User not found");
            return Result<UserDto>.Success(_mapper.Map<UserDto>(user));
        }

        public async Task<Result<UserDto>> UpdateProfileAsync(Guid userId, UpdateProfileDto dto, CancellationToken ct = default)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return Result<UserDto>.NotFound("User not found");

            if (dto.FirstName != null) user.FirstName = dto.FirstName;
            if (dto.LastName != null) user.LastName = dto.LastName;
            if (dto.PhoneNumber != null) user.PhoneNumber = dto.PhoneNumber;
            if (dto.ProfileImage != null) user.ProfileImage = dto.ProfileImage;
            user.UpdatedAt = DateTime.UtcNow;

            await _userManager.UpdateAsync(user);
            return Result<UserDto>.Success(_mapper.Map<UserDto>(user));
        }

        public async Task<AppResult> DeleteAccountAsync(Guid userId, string password, CancellationToken ct = default)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return AppResult.Failure("User not found", 404);
            if (!await _userManager.CheckPasswordAsync(user, password))
                return AppResult.Failure("Incorrect password", 401);

            await _userManager.DeleteAsync(user);
            return AppResult.Success("Account deleted");
        }

        public async Task<Result<PagedResult<UserDto>>> GetAllUsersAsync(PaginationQuery query, CancellationToken ct = default)
        {
            var usersQuery = _userManager.Users.Include(u => u.Wallets).Include(u => u.Transactions).AsQueryable();

            if (!string.IsNullOrEmpty(query.SearchTerm))
                usersQuery = usersQuery.Where(u =>
                    u.Email!.Contains(query.SearchTerm) ||
                    u.FirstName.Contains(query.SearchTerm) ||
                    u.LastName.Contains(query.SearchTerm) ||
                    u.UserName!.Contains(query.SearchTerm));

            var total = await usersQuery.CountAsync(ct);
            var users = await usersQuery
                .OrderByDescending(u => u.CreatedAt)
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync(ct);

            return Result<PagedResult<UserDto>>.Success(new PagedResult<UserDto>
            {
                Items = _mapper.Map<IEnumerable<UserDto>>(users),
                TotalCount = total,
                Page = query.Page,
                PageSize = query.PageSize
            });
        }

        public async Task<Result<UserDto>> GetUserByIdAsync(Guid userId, CancellationToken ct = default)
        {
            var user = await _userManager.Users
                .Include(u => u.Wallets)
                .Include(u => u.Transactions)
                .FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return Result<UserDto>.NotFound("User not found");
            return Result<UserDto>.Success(_mapper.Map<UserDto>(user));
        }

        public async Task<Result<UserDto>> AdminCreateUserAsync(AdminCreateUserDto dto, CancellationToken ct = default)
        {
            if (await _userManager.FindByEmailAsync(dto.Email) != null)
                return Result<UserDto>.Failure("Email already in use");

            var user = new AppUser
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                UserName = dto.UserName,
                Role = dto.Role,
                EmailVerified = true,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                return Result<UserDto>.Failure(result.Errors.Select(e => e.Description).ToList());

            return Result<UserDto>.Success(_mapper.Map<UserDto>(user), "User created", 201);
        }

        public async Task<Result<UserDto>> AdminUpdateUserAsync(Guid userId, AdminUpdateUserDto dto, CancellationToken ct = default)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return Result<UserDto>.NotFound("User not found");

            if (dto.FirstName != null) user.FirstName = dto.FirstName;
            if (dto.LastName != null) user.LastName = dto.LastName;
            if (dto.Role.HasValue) user.Role = dto.Role.Value;
            if (dto.IsActive.HasValue) user.IsActive = dto.IsActive.Value;

            await _userManager.UpdateAsync(user);
            await _auditService.LogAsync("AdminUpdateUser", "AppUser", userId.ToString(), newValues: dto);
            return Result<UserDto>.Success(_mapper.Map<UserDto>(user));
        }

        public async Task<AppResult> AdminDeleteUserAsync(Guid userId, CancellationToken ct = default)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return AppResult.Failure("User not found", 404);
            await _userManager.DeleteAsync(user);
            await _auditService.LogAsync("AdminDeleteUser", "AppUser", userId.ToString());
            return AppResult.Success("User deleted");
        }

        public async Task<AppResult> BanUserAsync(Guid userId, CancellationToken ct = default)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return AppResult.Failure("User not found", 404);
            user.IsActive = false;
            await _userManager.UpdateAsync(user);
            await _auditService.LogAsync("BanUser", "AppUser", userId.ToString());
            return AppResult.Success("User banned");
        }

        public async Task<AppResult> UnbanUserAsync(Guid userId, CancellationToken ct = default)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return AppResult.Failure("User not found", 404);
            user.IsActive = true;
            await _userManager.UpdateAsync(user);
            await _auditService.LogAsync("UnbanUser", "AppUser", userId.ToString());
            return AppResult.Success("User unbanned");
        }
    }
}
