using Blockchain.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blockchain.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Wallet> Wallets { get; }
        IGenericRepository<Transaction> Transactions { get; }
        IGenericRepository<Block> Blocks { get; }
        IGenericRepository<SmartContract> SmartContracts { get; }
        IGenericRepository<ContractInteraction> ContractInteractions { get; }
        IGenericRepository<AuditLog> AuditLogs { get; }
        IGenericRepository<Notification> Notifications { get; }

        Task<int> SaveChangesAsync(CancellationToken ct = default);
        Task BeginTransactionAsync(CancellationToken ct = default);
        Task CommitTransactionAsync(CancellationToken ct = default);
        Task RollbackTransactionAsync(CancellationToken ct = default);
    }
}
