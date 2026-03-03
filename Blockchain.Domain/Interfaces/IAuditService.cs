using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blockchain.Domain.Interfaces
{
    public interface IAuditService
    {
        Task LogAsync(string action, string entityName, string? entityId = null,
            object? oldValues = null, object? newValues = null,
            bool isSuccess = true, string? errorMessage = null,
            CancellationToken ct = default);
    }
}
