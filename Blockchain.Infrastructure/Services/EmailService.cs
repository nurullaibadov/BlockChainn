using Blockchain.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace Blockchain.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config) => _config = config;

        public async Task SendEmailAsync(string to, string subject, string htmlBody, CancellationToken ct = default)
        {
            var smtp = new SmtpClient(_config["EmailSettings:SmtpHost"])
            {
                Port = int.Parse(_config["EmailSettings:SmtpPort"]!),
                Credentials = new NetworkCredential(_config["EmailSettings:SmtpUser"], _config["EmailSettings:SmtpPass"]),
                EnableSsl = bool.Parse(_config["EmailSettings:EnableSsl"]!)
            };

            var mail = new MailMessage
            {
                From = new MailAddress(_config["EmailSettings:FromEmail"]!, _config["EmailSettings:FromName"]),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };
            mail.To.Add(to);

            await smtp.SendMailAsync(mail, ct);
        }

        public async Task SendEmailConfirmationAsync(string to, string token, CancellationToken ct = default)
        {
            var body = $@"
        <h2>Email Verification</h2>
        <p>Please verify your email by clicking the link below:</p>
        <a href='https://yourapi.com/api/auth/verify-email?email={to}&token={Uri.EscapeDataString(token)}'>Verify Email</a>
        <p>This link expires in 24 hours.</p>";
            await SendEmailAsync(to, "Verify Your Email", body, ct);
        }

        public async Task SendPasswordResetAsync(string to, string token, CancellationToken ct = default)
        {
            var body = $@"
        <h2>Password Reset</h2>
        <p>You requested a password reset. Click the link below:</p>
        <a href='https://yourapi.com/reset-password?email={to}&token={Uri.EscapeDataString(token)}'>Reset Password</a>
        <p>This link expires in 1 hour. If you didn't request this, ignore this email.</p>";
            await SendEmailAsync(to, "Reset Your Password", body, ct);
        }

        public async Task SendTransactionNotificationAsync(string to, string txHash, decimal amount, CancellationToken ct = default)
        {
            var body = $@"
        <h2>Transaction Confirmed</h2>
        <p>Your transaction of <strong>{amount} ETH</strong> has been confirmed.</p>
        <p>Transaction Hash: <code>{txHash}</code></p>";
            await SendEmailAsync(to, "Transaction Confirmed", body, ct);
        }

        public async Task SendWelcomeEmailAsync(string to, string firstName, CancellationToken ct = default)
        {
            var body = $@"
        <h2>Welcome, {firstName}!</h2>
        <p>Your BlockchainApi account has been created successfully.</p>
        <p>You can now create wallets and manage blockchain transactions.</p>";
            await SendEmailAsync(to, "Welcome to BlockchainApi", body, ct);
        }
    }
}
