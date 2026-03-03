using Blockchain.Application.Common;
using Blockchain.Domain.Entities;
using Blockchain.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blockchain.Application.Interfaces
{
    public interface INotificationService
    {
        Task CreateNotificationAsync(Guid userId, string title, string message,
            NotificationType type, string? relatedEntityId = null, CancellationToken ct = default);
        Task<Result<PagedResult<Notification>>> GetUserNotificationsAsync(
            Guid userId, PaginationQuery query, CancellationToken ct = default);
        Task<Result> MarkAsReadAsync(Guid notificationId, Guid userId, CancellationToken ct = default);
        Task<Result> MarkAllAsReadAsync(Guid userId, CancellationToken ct = default);
        Task<Result<int>> GetUnreadCountAsync(Guid userId, CancellationToken ct = default);
    }
}
