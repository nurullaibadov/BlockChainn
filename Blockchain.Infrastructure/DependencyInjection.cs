using Blockchain.Application.Interfaces;
using Blockchain.Domain.Entities;
using Blockchain.Domain.Interfaces;
using Blockchain.Infrastructure.Data;
using Blockchain.Infrastructure.Repositories;
using Blockchain.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blockchain.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            // Database
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(config.GetConnectionString("DefaultConnection"),
                    sql =>
                    {
                        sql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
                        sql.EnableRetryOnFailure(3, TimeSpan.FromSeconds(5), null);
                    }));

            // Identity
            services.AddIdentity<AppUser, IdentityRole<Guid>>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 8;
                options.User.RequireUniqueEmail = true;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                options.Lockout.MaxFailedAccessAttempts = 5;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

            // Redis Cache
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = config["Redis:ConnectionString"];
                options.InstanceName = config["Redis:InstanceName"];
            });

            // Repositories & UoW
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Domain Services
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IBlockchainService, BlockchainService>();
            services.AddScoped<ICacheService, CacheService>();
            services.AddScoped<IAuditService, AuditService>();
            services.AddScoped<EncryptionService>();

            // Application Services
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IWalletService, WalletService>();
            services.AddScoped<ITransactionService, TransactionService>();
            services.AddScoped<ISmartContractService, SmartContractService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IHubService, HubService>();

            services.AddHttpContextAccessor();

            return services;
        }
    }
}
