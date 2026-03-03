using AutoMapper;
using Blockchain.Application.DTOs.Auth;
using Blockchain.Application.DTOs.Contract;
using Blockchain.Application.DTOs.Transaction;
using Blockchain.Application.DTOs.User;
using Blockchain.Application.DTOs.Wallet;
using Blockchain.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blockchain.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // User
            CreateMap<AppUser, UserInfoDto>()
                .ForMember(d => d.Role, o => o.MapFrom(s => s.Role.ToString()));
            CreateMap<AppUser, UserDto>()
                .ForMember(d => d.WalletCount, o => o.MapFrom(s => s.Wallets.Count))
                .ForMember(d => d.TransactionCount, o => o.MapFrom(s => s.Transactions.Count));
            CreateMap<UpdateProfileDto, AppUser>()
                .ForAllMembers(o => o.Condition((src, dest, val) => val != null));

            // Wallet
            CreateMap<Wallet, WalletDto>();
            CreateMap<CreateWalletDto, Wallet>()
                .ForMember(d => d.Address, o => o.Ignore())
                .ForMember(d => d.EncryptedPrivateKey, o => o.Ignore())
                .ForMember(d => d.PublicKey, o => o.Ignore());

            // Transaction
            CreateMap<Transaction, TransactionDto>();

            // SmartContract
            CreateMap<SmartContract, SmartContractDto>();
            CreateMap<AddContractDto, SmartContract>();
        }
    }
}
