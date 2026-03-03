using Blockchain.Domain.Entities;
using Blockchain.Domain.Interfaces;
using Blockchain.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace Blockchain.Infrastructure.Services
{
    public class AuditService : IAuditService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuditService(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task LogAsync(string action, string entityName, string? entityId = null,
            object? oldValues = null, object? newValues = null,
            bool isSuccess = true, string? errorMessage = null, CancellationToken ct = default)
        {
            var httpCtx = _httpContextAccessor.HttpContext;
            var log = new AuditLog
            {
                Action = action,
                EntityName = entityName,
                EntityId = entityId,
                OldValues = oldValues != null ? JsonSerializer.Serialize(oldValues) : null,
                NewValues = newValues != null ? JsonSerializer.Serialize(newValues) : null,
                IsSuccess = isSuccess,
                ErrorMessage = errorMessage,
                IpAddress = httpCtx?.Connection.RemoteIpAddress?.ToString(),
                UserAgent = httpCtx?.Request.Headers["User-Agent"].ToString()
            };

            if (httpCtx?.User.Identity?.IsAuthenticated == true)
            {
                var userIdClaim = httpCtx.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                if (Guid.TryParse(userIdClaim?.Value, out var uid))
                    log.UserId = uid;
            }

            _context.AuditLogs.Add(log);
            await _context.SaveChangesAsync(ct);
        }
    }
}
