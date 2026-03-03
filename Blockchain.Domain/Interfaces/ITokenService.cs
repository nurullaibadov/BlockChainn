using Blockchain.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Blockchain.Domain.Interfaces
{
    public interface ITokenService
    {
        string GenerateAccessToken(AppUser user);
        string GenerateRefreshToken();
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
        Task<(string accessToken, string refreshToken)> RefreshTokenAsync(string accessToken, string refreshToken);
    }
}
