using Blockchain.Application.Common;
using Blockchain.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Blockchain.API.Controllers
{
    /// <summary>User notifications</summary>
    [Tags("Notifications")]
    [Authorize]
    public class NotificationController : BaseApiController
    {
        private readonly INotificationService _notifService;

        public NotificationController(INotificationService notifService) => _notifService = notifService;

        /// <summary>Get all notifications</summary>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginationQuery query, CancellationToken ct)
            => HandleResult(await _notifService.GetUserNotificationsAsync(CurrentUserId, query, ct));

        /// <summary>Get unread count</summary>
        [HttpGet("unread-count")]
        public async Task<IActionResult> UnreadCount(CancellationToken ct)
            => HandleResult(await _notifService.GetUnreadCountAsync(CurrentUserId, ct));

        /// <summary>Mark notification as read</summary>
        [HttpPost("{id:guid}/read")]
        public async Task<IActionResult> MarkRead(Guid id, CancellationToken ct)
            => HandleResult(await _notifService.MarkAsReadAsync(id, CurrentUserId, ct));

        /// <summary>Mark all as read</summary>
        [HttpPost("read-all")]
        public async Task<IActionResult> MarkAllRead(CancellationToken ct)
            => HandleResult(await _notifService.MarkAllAsReadAsync(CurrentUserId, ct));
    }
}
