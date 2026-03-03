using Blockchain.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace Blockchain.Domain.Entities
{
    public class AppUser : IdentityUser<Guid>
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? ProfileImage { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }
        public string? PasswordResetToken { get; set; }
        public DateTime? PasswordResetTokenExpiry { get; set; }
        public string? EmailVerificationToken { get; set; }
        public bool EmailVerified { get; set; } = false;
        public UserRole Role { get; set; } = UserRole.User;

        // Navigation
        public ICollection<Wallet> Wallets { get; set; } = new List<Wallet>();
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
