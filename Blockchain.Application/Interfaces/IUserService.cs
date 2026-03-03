using Blockchain.Application.Common;
using Blockchain.Application.DTOs.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blockchain.Application.Interfaces
{
    public interface IUserService
    {
        Task<Result<UserDto>> GetProfileAsync(Guid userId, CancellationToken ct = default);
        Task<Result<UserDto>> UpdateProfileAsync(Guid userId, UpdateProfileDto dto, CancellationToken ct = default);
        Task<Result> DeleteAccountAsync(Guid userId, string password, CancellationToken ct = default);
        Task<Result<PagedResult<UserDto>>> GetAllUsersAsync(PaginationQuery query, CancellationToken ct = default);
        Task<Result<UserDto>> GetUserByIdAsync(Guid userId, CancellationToken ct = default);
        Task<Result<UserDto>> AdminCreateUserAsync(AdminCreateUserDto dto, CancellationToken ct = default);
        Task<Result<UserDto>> AdminUpdateUserAsync(Guid userId, AdminUpdateUserDto dto, CancellationToken ct = default);
        Task<Result> AdminDeleteUserAsync(Guid userId, CancellationToken ct = default);
        Task<Result> BanUserAsync(Guid userId, CancellationToken ct = default);
        Task<Result> UnbanUserAsync(Guid userId, CancellationToken ct = default);
    }
}
