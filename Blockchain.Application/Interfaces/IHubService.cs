using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blockchain.Application.Interfaces
{
    public interface IHubService
    {
        Task SendNotificationAsync(string userId, string title, string message, object? data = null);
        Task SendTransactionUpdateAsync(string userId, string txHash, string status);
        Task SendBalanceUpdateAsync(string userId, string walletAddress, decimal newBalance);
    }
}
