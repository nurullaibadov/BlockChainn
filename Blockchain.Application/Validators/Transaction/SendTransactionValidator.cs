using Blockchain.Application.DTOs.Transaction;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blockchain.Application.Validators.Transaction
{
    public class SendTransactionValidator : AbstractValidator<SendTransactionDto>
    {
        public SendTransactionValidator()
        {
            RuleFor(x => x.FromWalletId).NotEmpty();
            RuleFor(x => x.ToAddress).NotEmpty().Length(42, 42)
                .Matches("^0x[a-fA-F0-9]{40}$").WithMessage("Invalid Ethereum address format");
            RuleFor(x => x.Amount).GreaterThan(0);
        }
    }
}
