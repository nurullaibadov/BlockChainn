using Blockchain.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blockchain.Application.DTOs.User
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public UserRole Role { get; set; }
        public bool IsActive { get; set; }
        public bool EmailVerified { get; set; }
        public string? ProfileImage { get; set; }
        public DateTime CreatedAt { get; set; }
        public int WalletCount { get; set; }
        public int TransactionCount { get; set; }
    }

    public class UpdateProfileDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? ProfileImage { get; set; }
    }

    public class AdminUpdateUserDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public UserRole? Role { get; set; }
        public bool? IsActive { get; set; }
    }

    public class AdminCreateUserDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public UserRole Role { get; set; } = UserRole.User;
    }
}
