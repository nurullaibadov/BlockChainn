using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace Blockchain.API.Hubs
{
    [Authorize]
    public class NotificationHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != null)
                await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != null)
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user_{userId}");
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendToUser(string userId, string title, string message)
            => await Clients.Group($"user_{userId}").SendAsync("ReceiveNotification", new { title, message, timestamp = DateTime.UtcNow });
    }
}
