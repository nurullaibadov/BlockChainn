using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blockchain.Domain.Interfaces
{
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string key, CancellationToken ct = default);
        Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken ct = default);
        Task RemoveAsync(string key, CancellationToken ct = default);
        Task<bool> ExistsAsync(string key, CancellationToken ct = default);
        Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiry = null, CancellationToken ct = default);
    }
}
