using Blockchain.Application.Common;
using Blockchain.Application.Interfaces;
using Blockchain.Domain.Entities;
using Blockchain.Domain.Enums;
using Blockchain.Domain.Interfaces;
using AppResult = Blockchain.Application.Common.Result;

namespace Blockchain.Infrastructure.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _uow;

        public NotificationService(IUnitOfWork uow) => _uow = uow;

        public async Task CreateNotificationAsync(Guid userId, string title, string message,
            NotificationType type, string? relatedEntityId = null, CancellationToken ct = default)
        {
            var notification = new Notification
            {
                UserId = userId,
                Title = title,
                Message = message,
                Type = type,
                RelatedEntityId = relatedEntityId
            };
            await _uow.Notifications.AddAsync(notification, ct);
            await _uow.SaveChangesAsync(ct);
        }

        public async Task<Result<PagedResult<Notification>>> GetUserNotificationsAsync(Guid userId, PaginationQuery query, CancellationToken ct = default)
        {
            var (items, total) = await _uow.Notifications.GetPagedAsync(
                query.Page, query.PageSize,
                n => n.UserId == userId && !n.IsDeleted,
                q => q.OrderByDescending(n => n.CreatedAt), ct);

            return Result<PagedResult<Notification>>.Success(new PagedResult<Notification>
            {
                Items = items,
                TotalCount = total,
                Page = query.Page,
                PageSize = query.PageSize
            });
        }

        public async Task<AppResult> MarkAsReadAsync(Guid notificationId, Guid userId, CancellationToken ct = default)
        {
            var n = await _uow.Notifications.FirstOrDefaultAsync(x => x.Id == notificationId && x.UserId == userId, ct);
            if (n == null) return AppResult.Failure("Notification not found", 404);
            n.IsRead = true;
            await _uow.Notifications.UpdateAsync(n, ct);
            await _uow.SaveChangesAsync(ct);
            return AppResult.Success();
        }

        public async Task<AppResult> MarkAllAsReadAsync(Guid userId, CancellationToken ct = default)
        {
            var notifications = await _uow.Notifications.FindAsync(n => n.UserId == userId && !n.IsRead, ct);
            foreach (var n in notifications) { n.IsRead = true; await _uow.Notifications.UpdateAsync(n, ct); }
            await _uow.SaveChangesAsync(ct);
            return AppResult.Success();
        }

        public async Task<Result<int>> GetUnreadCountAsync(Guid userId, CancellationToken ct = default)
        {
            var count = await _uow.Notifications.CountAsync(n => n.UserId == userId && !n.IsRead, ct);
            return Result<int>.Success(count);
        }
    }
}
