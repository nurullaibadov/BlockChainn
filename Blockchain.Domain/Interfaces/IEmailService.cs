using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blockchain.Domain.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string htmlBody, CancellationToken ct = default);
        Task SendEmailConfirmationAsync(string to, string token, CancellationToken ct = default);
        Task SendPasswordResetAsync(string to, string token, CancellationToken ct = default);
        Task SendTransactionNotificationAsync(string to, string txHash, decimal amount, CancellationToken ct = default);
        Task SendWelcomeEmailAsync(string to, string firstName, CancellationToken ct = default);
    }
}
