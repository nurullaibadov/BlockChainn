using Blockchain.Application.DTOs.Auth;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blockchain.Application.Validators.Auth
{
    public class RegisterValidator : AbstractValidator<RegisterDto>
    {
        public RegisterValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().MaximumLength(50);
            RuleFor(x => x.LastName).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.UserName).NotEmpty().MinimumLength(3).MaximumLength(30)
                .Matches("^[a-zA-Z0-9_]+$").WithMessage("Username can only contain letters, numbers and underscore");
            RuleFor(x => x.Password).NotEmpty().MinimumLength(8)
                .Matches("[A-Z]").WithMessage("Password must have at least one uppercase letter")
                .Matches("[a-z]").WithMessage("Password must have at least one lowercase letter")
                .Matches("[0-9]").WithMessage("Password must have at least one number")
                .Matches("[^a-zA-Z0-9]").WithMessage("Password must have at least one special character");
            RuleFor(x => x.ConfirmPassword).Equal(x => x.Password).WithMessage("Passwords do not match");
        }
    }
}
