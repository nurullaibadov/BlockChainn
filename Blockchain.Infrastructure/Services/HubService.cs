using Blockchain.Application.Interfaces;
using Blockchain.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Blockchain.Infrastructure.Services
{
    public class HubService : IHubService
    {
        private readonly IHubContext<NotificationHub> _hub;

        public HubService(IHubContext<NotificationHub> hub) => _hub = hub;

        public async Task SendNotificationAsync(string userId, string title, string message, object? data = null)
            => await _hub.Clients.Group($"user_{userId}")
                .SendAsync("ReceiveNotification", new { title, message, data, timestamp = DateTime.UtcNow });

        public async Task SendTransactionUpdateAsync(string userId, string txHash, string status)
            => await _hub.Clients.Group($"user_{userId}")
                .SendAsync("TransactionUpdate", new { txHash, status, timestamp = DateTime.UtcNow });

        public async Task SendBalanceUpdateAsync(string userId, string walletAddress, decimal newBalance)
            => await _hub.Clients.Group($"user_{userId}")
                .SendAsync("BalanceUpdate", new { walletAddress, newBalance, timestamp = DateTime.UtcNow });
    }
}
